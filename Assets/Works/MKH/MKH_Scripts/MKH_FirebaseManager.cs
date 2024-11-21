using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MKH_FirebaseManager : MonoBehaviour
{
   public static MKH_FirebaseManager Instance { get; private set; }

    private FirebaseApp app;
    public static FirebaseApp App { get { return Instance.app; } }

    private FirebaseAuth auth;
    public static FirebaseAuth Auth { get { return Instance.auth; } }

    private void Awake()
    {
        CreateSingleton();
    }

    private void Start()
    {
        CheckDependency();
    }

    private void CreateSingleton()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CheckDependency()
    {
        //                  ��û                          ����
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                Debug.Log("Firebase dependencies check success");
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");
                // Firebase Unity SDK is not safe to use here.
                app = null;
                auth = null;
            }
        });
    }
}
