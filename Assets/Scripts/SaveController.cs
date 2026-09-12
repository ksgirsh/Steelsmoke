using UnityEngine;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SaveController : MonoBehaviour
{

    private string saveLocation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Define save location
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        Debug.Log(saveLocation);
    }

    public void SaveGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        SaveData data = new SaveData
        {

            playerPosition = player.transform.position,
            // mapBoundary = FindObjectOfType<CinemachineConfiner>().m_BoundingShape2D.gameObject.name,
            currHealth = player.GetComponent<Health>().currentHealth,
            maxHealth = player.GetComponent<Health>().setHealth,
            fuel = player.GetComponent<RunAndJump>().fuel,
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(data));
    }

    public void LoadGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (File.Exists(saveLocation))
        {
            SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            player.transform.position = data.playerPosition;
            player.GetComponent<Health>().currentHealth = data.currHealth;
            player.GetComponent<Health>().setHealth = data.maxHealth;
            player.GetComponent<RunAndJump>().fuel = data.fuel;
            player.GetComponent<Respawn>().ResetEnemies();
        }
        else
        {
            SaveGame();
        }
    }
}
