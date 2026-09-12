using UnityEngine;
using UnityEngine.Rendering;

public class SmokeEmitter : MonoBehaviour
{
    [Header("Grid Size")]
    public int GridSize = 64;
    //world size of each cell
    public float CellSize = 0.1f;

    public Collider2D smokeRegion;
    private bool[,] blocked;
    private Bounds bounds;


    [Header("Fluid Properties")]
    [SerializeField] float diffusion = 0.0001f;
    [SerializeField] float viscosity = 0.0001f;
    [SerializeField] float timeStep = 0.0167f;

    [Header("Smoke Properties")]
    [SerializeField] float smokeDecay;
    [SerializeField] float emissionStrength;
    [SerializeField] Vector2 emissionPoint = new Vector2(32f, 10f);
    [SerializeField] float emissionRadius;
    [SerializeField] Vector2 emissionVelocity = new Vector2(0f, 20f);
    [SerializeField] float velocityRandomness = 50f;

    [SerializeField] Material smokeMaterial;
    [SerializeField] Sprite sprite;
    [SerializeField] int sortingOrder;

    [SerializeField] int bits = 16;
    private Renderer objRenderer;

    [Header("Visualization")]
    float[,] density, prevDensity;
    float[,] velocityX, prevVelocityX;
    float[,] velocityY, prevVelocityY;

    private Texture2D smokeTexture;
    private GameObject smokeSpriteObj;

    private float emissionRadiusSquared;
    private int smokeTextureProperty = Shader.PropertyToID("_SmokeTexture");
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        emissionRadiusSquared = emissionRadius * emissionRadius;
        if (smokeRegion != null)
        {
            bounds = smokeRegion.bounds; 
          //  GridSize = Mathf.CeilToInt(regionBounds.size.x / CellSize);
        }

        InitializeFluidArrays();
        CreateVisualization();

        objRenderer = gameObject.GetComponentInChildren<Renderer>();
        objRenderer.material.SetFloat("_Bits", bits);
    }

    void InitializeFluidArrays()
    {
        int size = GridSize + 2;

        density = new float[size, size];
        prevDensity = new float[size, size];
        velocityX = new float[size, size];
        prevVelocityX = new float[size, size];
        velocityY = new float[size, size];
        prevVelocityY = new float[size, size];



    }

    void CreateVisualization()
    {
        smokeTexture = new Texture2D(GridSize, GridSize, TextureFormat.RGBA32, false);
        smokeTexture.filterMode = FilterMode.Bilinear;

        smokeSpriteObj = new GameObject("Smoke Display");
        smokeSpriteObj.transform.parent = transform;
        smokeSpriteObj.transform.localPosition = Vector3.zero;
        smokeSpriteObj.transform.localScale = new Vector3(GridSize * CellSize, GridSize * CellSize, 1f);

        SpriteRenderer sr = smokeSpriteObj.AddComponent<SpriteRenderer>();
        sr.material = new Material(smokeMaterial);
        sr.material.SetTexture(smokeTextureProperty, smokeTexture);
        sr.sprite = sprite;
        sr.sortingOrder = sortingOrder;


        objRenderer = smokeSpriteObj.GetComponentInChildren<Renderer>();
        objRenderer.material.SetFloat("_Bits", bits);
    }


    // Update is called once per frame
    void Update()
    {
        AddSmokeSource();
        VelocityUpdate();
        DensityUpdate();
        SmokeTextureUpdate();
    }

    void AddSmokeSource()
    {
        int centerX = Mathf.RoundToInt(emissionPoint.x);
        int centerY = Mathf.RoundToInt(emissionPoint.y);
        int radius = Mathf.RoundToInt(emissionRadius);

        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                float distanceSquared = ((i * i) + (j * j));
                if (distanceSquared > emissionRadiusSquared)
                {
                    continue;
                }

                int x = centerX + i;
                int y = centerY + j;

                if (x >= 1 && x <= GridSize && y >= 1 && y <= GridSize)
                {
                    float distance = Mathf.Sqrt(distanceSquared);
                    float falloff = 1f - (distance / emissionRadius);
                    density[x, y] += emissionStrength * falloff * timeStep;

                    float randomVelX = Random.Range(-velocityRandomness, velocityRandomness);
                    float randomVelY = Random.Range(-velocityRandomness, velocityRandomness);


                    velocityX[x, y] += ((randomVelX + emissionVelocity.x) * falloff * timeStep);
                    velocityY[x, y] += ((randomVelY + emissionVelocity.y) * falloff * timeStep);
                }
                     
            }
        }

    }

    public void SetBlockedCell(int x, int y)
    {
        
        density[x, y] = 0;
        prevDensity[x, y] = 0;

        velocityX[x, y] = 0;
        prevVelocityX[x, y] = 0;

        velocityY[x, y] = 0;
        prevVelocityY[x, y] = 0;
        

    }

    void VelocityUpdate()
    {
        System.Array.Copy(velocityX, prevVelocityX, velocityX.Length);
        System.Array.Copy(velocityY, prevVelocityY, velocityY.Length);

        Diffuse(1, velocityX, prevVelocityX, viscosity, timeStep);
        Diffuse(2, velocityY, prevVelocityY, viscosity, timeStep);

        System.Array.Copy(velocityX, prevVelocityX, velocityX.Length);
        System.Array.Copy(velocityY, prevVelocityY, velocityY.Length);

        Advect(1, velocityX, prevVelocityX, prevVelocityX, prevVelocityY, timeStep);
        Advect(2, velocityY, prevVelocityY, prevVelocityX, prevVelocityY, timeStep);

        Project(velocityX, velocityY, prevVelocityX, prevVelocityY);
    }

    void DensityUpdate()
    {
        System.Array.Copy(density, prevDensity, density.Length);
        Diffuse(0, density, prevDensity, diffusion, timeStep);

        System.Array.Copy(density, prevDensity, density.Length);
        Advect(0, density, prevDensity, velocityX, velocityY, timeStep);

        for (int i = 1; i <= GridSize; i++)
        {
            for (int j = 1; j <= GridSize; j++)
            {
                density[i, j] *= smokeDecay;
            }
        }
    }

    void SmokeTextureUpdate()
    {
        Color[] colors = new Color[GridSize * GridSize];
        for (int i = 0; i < GridSize; i++)
        {
            for (int j = 0; j < GridSize; j++)
            {
                float densityValue = Mathf.Clamp01(density[i + 1, j + 1] / 10f);

                colors[(j * GridSize) + i] = new Color(densityValue, densityValue, densityValue, densityValue);
            }
        }

        objRenderer = gameObject.GetComponentInChildren<Renderer>();
        objRenderer.material.SetFloat("_Bits", bits);

        smokeTexture.SetPixels(colors);
        smokeTexture.Apply();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 emissionWorldPoint = transform.position + new Vector3(((emissionPoint.x - (GridSize * 0.5f)) * CellSize), ((emissionPoint.y - (GridSize * 0.5f)) * CellSize), 0f);
        Gizmos.DrawWireSphere(emissionWorldPoint, emissionRadius * CellSize);
    }


    void Diffuse(int b, float[,] x, float[,] x0, float diff, float dt)
    {

        float a = dt * diff * GridSize * GridSize;
        LinearSolve(b, x, x0, a, (1 + 4 * a));
    }

    void Advect(int b, float[,] d, float[,] d0, float[,] u, float[,] v, float dt)
    {
        //int i, j, i0, j0, i1, j1;

        float dt0 = dt * GridSize;
        for (int i = 1; i <= GridSize; i++)
        {
            for (int j = 1; j <= GridSize; j++)
            {
                float x = i - dt0 * u[i, j]; 
                float y = j - dt0 * v[i, j];

                x = Mathf.Clamp(x, 0.5f, GridSize + 0.5f);
                y = Mathf.Clamp(y, 0.5f, GridSize + 0.5f);

                int i0 = (int)x;
                int i1 = i0 + 1;
                int j0 = (int)y;
                int j1 = j0 + 1;

                float s1 = x - i0;
                float s0 = 1 - s1;
                float t1 = y - j0;
                float t0 = 1 - t1;

                
                d[i, j] = s0 * (t0 * d0[i0, j0] + t1 * d0[i0, j1]) + s1 * (t0 * d0[i1, j0] + t1 * d0[i1, j1]);

            }
        }

        SetBoundary(b, d);
    }


    void LinearSolve(int b, float[,] x, float[,] x0, float a, float c)
    {
        int i, j, k;
        for (k = 0; k < 20; k++)
        {
            for (i = 1; i <= GridSize; i++)
            {
                for (j = 1; j <= GridSize; j++)
                {
                    x[i, j] = (x0[i, j] + a * (x[i - 1, j] + x[i + 1, j] + x[i, j - 1] + x[i, j + 1])) / c;
                    //c used to equal: (1 + 4 * a)
                }
            }
            SetBoundary(b, x);
        }
    }

    void Project(float[,] u, float[,] v, float[,] p, float[,] div)
    {
        int i, j, k;

        float h;
        h = 1.0f / GridSize;
        for (i = 1; i <= GridSize; i++)
        {
            for (j = 1; j <= GridSize; j++)
            {
                div[i, j] = -0.5f * h * (u[i + 1, j] - u[i - 1, j] + v[i, j + 1] - v[i, j - 1]);
                p[i, j] = 0;
            }
        }

        SetBoundary(0, div); 
        SetBoundary(0, p);

        /*
        for (k = 0; k < 20; k++)
        {
            for (i = 1; i <= GridSize; i++)
            {
                for (j = 1; j <= GridSize; j++)
                {
                    p[i, j] = (div[i, j] + p[i - 1, j] + p[i + 1, j] +
                    p[i, j - 1] + p[i, j + 1]) / 4;
                }
            }
            set_bnd(0, p);
        }*/

        LinearSolve(0, p, div, 1, 4);

        for (i = 1; i <= GridSize; i++)
        {
            for (j = 1; j <= GridSize; j++)
            {
                u[i, j] -= 0.5f * (p[i + 1, j] - p[i - 1, j]) / h;
                v[i, j] -= 0.5f * (p[i, j + 1] - p[i, j - 1]) / h;
            }
        }
        SetBoundary(1, u); 
        SetBoundary(2, v);
    }


    void SetBoundary(int b, float[,] x )
    {
        int N = GridSize;
        for (int i = 1; i <= N; i++)
        {
            x[0, i] = b == 1 ? -x[1, i] : x[1, i];

            x[N + 1, i] = b == 1 ? -x[N, i] : x[N, i];
            x[i, 0] = b == 2 ? -x[i, 1] : x[i, 1];
            x[i, N + 1] = b == 2 ? -x[i, N] : x[i, N];
        }
        x[0, 0] = 0.5f * (x[1, 0] + x[0, 1]);
        x[0, N + 1] = 0.5f * (x[1, N + 1] + x[0, N]);
        x[N + 1, 0] = 0.5f * (x[N, 0] + x[N + 1, 1]);
        x[N + 1, N + 1] = 0.5f * (x[N, N + 1] + x[N + 1, N]);
    }
}
