using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Schools")]
    [SerializeField] private List<GameObject> schools = new();
    public static event Action OnLevelComplete;

    [Header("Audio")]
    [SerializeField] private AudioClip BGM;
    [SerializeField] private AudioClip ambient;

    [SerializeField] private bool hasHUD = true;

    public bool HasHUD => hasHUD;

    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        //Check for multiple level managers in scene
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple LevelManagers in scene!");
        }

        //Assign singleton instance
        Instance = this;
    }
    private void Start()
    {
        schools.AddRange(GameObject.FindGameObjectsWithTag("School"));

        //Assign LevelManager reference to each school
        foreach (var school in schools)
        {
            SchoolController schoolController = school.GetComponent<SchoolController>();
            if (schoolController != null)
            {
                schoolController.LevelManager = this;
            }
        }

        //Play level audio
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(BGM);
            AudioManager.Instance.PlayAmbient(ambient);
        }
    }

    public void RemoveSchool(GameObject school)
    {
        if (schools.Contains(school))
        {
            schools.Remove(school);
        }

        //Check win condition
        if (schools.Count <= 0)
        {
            OnLevelComplete?.Invoke();
        }
    }
}
