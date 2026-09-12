using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class Ultimate : MonoBehaviour
{
    [SerializeField] RunAndJump rj;
    [SerializeField] Health heal;
    [SerializeField] Shooting shoot;

    [field: SerializeField] public bool ultimateState { get; private set; }
    [SerializeField] float ultimateDuration;
    
    [SerializeField] float ultimateSpeedBoost;
    [field: SerializeField] public float ultimateAttackSpeed { get; private set; }
    [field: SerializeField] public Color ultFuelColor { get; private set; }

    public float speedBoost { get; private set; }
    [SerializeField] Image fuelR;
    [SerializeField] float shakeIntensity;
    [SerializeField] Color tint;
    [SerializeField] GameObject tintObject;
    [SerializeField] Light2D ultiLight;

    [SerializeField] ParticleSystem ultParticles1;
    [SerializeField] ParticleSystem ultParticles2;

    [SerializeField] TrailRenderer ultTrail1 = null;

    [SerializeField] float ultLightIntensity;

    [SerializeField] AudioClip[] ultimates;

    // Start is called before the first frame update
    void Start()
    {
        ultimateState = false;
        speedBoost = 0;
        tintObject.SetActive(false);

        shoot = gameObject.GetComponent<Shooting>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rj.fuel > (rj.maxFuel - 0.001) && Input.GetKeyDown(KeyCode.Q) && ultimateState == false)
        {
            rj.AddFuel(0.01f);
            StartCoroutine(DrainFuel());
            StartCoroutine(ActivateUlt());
            /*
            StartCoroutine(ShakeEff());
            StartCoroutine(LightEff());
            StartCoroutine(ParticleEff());
            */
        }
    }

    IEnumerator ActivateUlt()
    {
        SoundFXManager.instance.PlaySoundEffectClip(ultimates[0], transform.position, 1f);
        tintObject.GetComponent<Image>().color = tint;
        tintObject.SetActive(true);
       
        /*

        fuelR.material.SetColor("_Color2", ultFuelColor);
        fuelR.material.SetColor("_Color1", Color.gray);
        */

        ultimateState = true;
        speedBoost = ultimateSpeedBoost;

        yield return null;


    }

    public IEnumerator DrainFuel()
    {
      
        Color initColor2 = fuelR.material.GetColor("_Color2");
        Color initColor1 = fuelR.material.GetColor("_Color1");

        float initialFuel = rj.fuel;

        while (rj.fuel > 0f)
        {
            float t = 0f;
            t += Time.deltaTime;
            float normalizedTime = t / ultimateDuration;


            float lerpFuel = Mathf.Lerp(rj.maxFuel, 0, normalizedTime);
            rj.fuel -= (Time.deltaTime/ultimateDuration);
            yield return null;
        }

        
        speedBoost = 0;

        fuelR.material.SetColor("_Color1", initColor1);
        fuelR.material.SetColor("_Color2", initColor2);

        tintObject.SetActive(false);
        //ReplenishHealth(1);

        //allow overheal of 1
        if (heal.currentHealth > 0 && heal.currentHealth < (heal.setHealth + 1))
        {
            StartCoroutine(heal.TakePDamage(-1, Vector2.zero));
        }

        if (shoot.indicatorExists)
        {
            StartCoroutine(shoot.UltRamInit());
        }

        rj.AddFuel(0f);
        ultParticles2.Emit(8);


        //for any exit animations
        yield return new WaitForSeconds(1f);
       
        ultimateState = false;
    }

    IEnumerator LightEff()
    {

        LightControl pL = ultiLight.GetComponent<LightControl>();

        //Light2D pL2 = ultiLight.GetComponentInChildren<Light2D>();

        //StartCoroutine(lc.Pulse(ultimateDuration, 2.667f));
        pL.SetColor(ultFuelColor);

        while (rj.fuel > 0f)
        {

         
            ultiLight.intensity = ultLightIntensity;
            yield return null;
        }

        StartCoroutine(pL.Pulse(0.2f, ultLightIntensity));
       // pL2.gameObject.SetActive(false);
      
      
      
    }

    IEnumerator ShakeEff()
    {
      
        var fuelBar = fuelR.gameObject.transform.parent;

        var initialPos = fuelBar.position;
        Debug.Log(initialPos);

        var maxTrembleTime = ultimateDuration;
        var currentTime = 0.0f;

        while (rj.fuel > 0f)
        {
            currentTime = (currentTime + Time.deltaTime * 10);
     

            Vector3 move = Random.insideUnitCircle * 10;
            Vector3 dir = move.normalized;

            fuelBar.position += (dir / 10f) * shakeIntensity;
            yield return new WaitForSeconds(Time.deltaTime);
            fuelBar.position -= (dir / 10f) * shakeIntensity;

       

            yield return null; // wait until next frame
        }
    }

    IEnumerator Shake()
    {
        var fuelBar = fuelR.gameObject.transform.parent;

        var initialPos = fuelBar.position;

        for (float i = 0; i > rj.maxFuel; i += Time.deltaTime)
        {
            Vector3 move = Random.insideUnitCircle * 10;
            Vector3 dir = move.normalized;

            fuelBar.position += (dir / 10f) * shakeIntensity;
            yield return new WaitForSeconds(Time.deltaTime);
            fuelBar.position -= (dir / 10f) * shakeIntensity;

            yield return null;
        }

    }

    IEnumerator ParticleEff()
    {
        while(rj.fuel > 0f)
        {
           
            ultParticles1.Emit(1);
            yield return new WaitForSeconds(0.1f);
            
        
        }
    }

    void OnEnable()
    {
        Respawn.OnRespawnEvent += LoadValues;
    }

    void LoadValues(Respawn resp)
    {

        ultimateState = false;
    }


}
