using Il2CppRUMBLE.Interactions.InteractionBase;
using Il2CppRUMBLE.Managers;
using Il2CppRUMBLE.MoveSystem;
using Il2CppRUMBLE.Players.Subsystems;
using Il2CppTMPro;
using MelonLoader;
using RumbleModdingAPI;
using RumbleModUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VoidLand
{
    public class main : MelonMod
    {
        private string currentScene = "Loader";
        private bool gymInit = false;
        private bool voidLandActive = false;
        private bool respawning = false;
        private GameObject buttonToSwaptoVoidLand, voidLandTextPanel, voidLandParent, cube;
        private List<string> dontDisableGameObject = new List<string>();
        private List<GameObject> DisabledDDOLGameObjects = new List<GameObject>();
        private GameObject plane;
        UI UI = UI.instance;
        private Mod VoidLand = new Mod();
        private int size;
        private float r, g, b;
        private bool returnToGym;

        public override void OnLateInitializeMelon()
        {
            VoidLand.ModName = "VoidLand";
            VoidLand.ModVersion = "1.0.0";
            VoidLand.SetFolder("VoidLand");
            VoidLand.AddToList("Map Size", 125, "Determins the size of the VoidLand", new Tags { });
            VoidLand.AddToList("Return to Gym", false, 0, "Loads from VoidLand to Gym", new Tags { DoNotSave = true });
            VoidLand.AddToList("Color R", 0, "R Value out of 255", new Tags { });
            VoidLand.AddToList("Color G", 0, "G Value out of 255", new Tags { });
            VoidLand.AddToList("Color B", 0, "B Value out of 255", new Tags { });
            VoidLand.GetFromFile();
            VoidLand.ModSaved += Save;
            size = (int)VoidLand.Settings[0].SavedValue;
            if (size < 1)
            {
                size = 1;
                VoidLand.Settings[0].SavedValue = 1;
                VoidLand.Settings[0].Value = 1;
            }
            returnToGym = (bool)VoidLand.Settings[1].SavedValue;
            r = ((float)(int)VoidLand.Settings[2].SavedValue) / 255;
            g = ((float)(int)VoidLand.Settings[3].SavedValue) / 255;
            b = ((float)(int)VoidLand.Settings[4].SavedValue) / 255;
            UI.instance.UI_Initialized += UIInit;
            Calls.onMapInitialized += SceneInit;
            dontDisableGameObject.Add("LanguageManager");
            dontDisableGameObject.Add("PhotonMono");
            dontDisableGameObject.Add("Game Instance");
            dontDisableGameObject.Add("Timer Updater");
            dontDisableGameObject.Add("PlayFabHttp");
            dontDisableGameObject.Add("LIV");
            dontDisableGameObject.Add("UniverseLibCanvas");
            dontDisableGameObject.Add("UE_Freecam");
            dontDisableGameObject.Add("--------------SCENE--------------");
            dontDisableGameObject.Add("!ftraceLightmaps");
            dontDisableGameObject.Add("VoiceLogger");
            dontDisableGameObject.Add("Player Controller(Clone)");
            dontDisableGameObject.Add("Health");
        }

        private void Save()
        {
            size = (int)VoidLand.Settings[0].SavedValue;
            if (size < 1)
            {
                size = 1;
                VoidLand.Settings[0].SavedValue = 1;
                VoidLand.Settings[0].Value = 1;
            }
            r = ((float)(int)VoidLand.Settings[2].SavedValue) / 255;
            g = ((float)(int)VoidLand.Settings[3].SavedValue) / 255;
            b = ((float)(int)VoidLand.Settings[4].SavedValue) / 255;
            if (voidLandActive && (bool)VoidLand.Settings[1].SavedValue)
            {
                VoidLand.Settings[1].Value = false;
                ReLoadGym();
                return;
            }
            if (voidLandActive)
            {
                MelonCoroutines.Start(ReLoadVoidLand());
            }
        }

        private void UIInit()
        {
            UI.AddMod(VoidLand);
        }

        public override void OnFixedUpdate()
        {
            if (voidLandActive)
            {
                float playerY = PlayerManager.instance.localPlayer.Controller.gameObject.transform.GetChild(2).GetChild(13).GetChild(0).position.y;
                if (!respawning && (playerY <= -10))
                {
                    respawning = true;
                    PlayerManager.instance.localPlayer.Controller.gameObject.GetComponent<PlayerResetSystem>().ResetPlayerController();
                }
                else if (respawning && (playerY >= -10))
                {
                    respawning = false;
                }
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            currentScene = sceneName;
            if (voidLandActive)
            {
                ReactivateDDOLObjects();
                voidLandActive = false;
            }
            gymInit = false;
        }

        private void SceneInit()
        {
            if ((currentScene == "Gym") && !gymInit)
            {
                GameObject matchmakerBackPanel = GameObject.CreatePrimitive(PrimitiveType.Cube);
                matchmakerBackPanel.name = "BackPanel";
                matchmakerBackPanel.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
                matchmakerBackPanel.GetComponent<Renderer>().material.color = new Color(0, 1, 0.814f);
                matchmakerBackPanel.transform.parent = Calls.GameObjects.Gym.Logic.HeinhouserProducts.MatchConsole.GetGameObject().transform;
                matchmakerBackPanel.transform.localRotation = Quaternion.Euler(0, 0, 0);
                matchmakerBackPanel.transform.localPosition = new Vector3(-0.4915f, 1.2083f, -0.0151f);
                matchmakerBackPanel.transform.localScale = new Vector3(3f, 2.41f, 0.02f);
                voidLandParent = new GameObject();
                voidLandParent.name = "VoidLand";
                voidLandTextPanel = GameObject.Instantiate(Calls.GameObjects.Gym.Logic.HeinhouserProducts.MatchConsole.RankRelaxControls.GetGameObject().transform.GetChild(17).gameObject);
                voidLandTextPanel.name = "VoidLand Plate";
                voidLandTextPanel.transform.parent = voidLandParent.transform;
                voidLandTextPanel.transform.position = new Vector3(7.45f, 1.9f, 10.12f);
                voidLandTextPanel.transform.rotation = Quaternion.Euler(90f, 122.8f, 0f);
                voidLandTextPanel.transform.localScale = new Vector3(0.29f, 0.3036f, 0.362f);
                GameObject textPanelTextGO = GameObject.Instantiate(Calls.GameObjects.Gym.Logic.HeinhouserProducts.MatchConsole.RankRelaxControls.GetGameObject().transform.GetChild(15).GetChild(6).gameObject);
                textPanelTextGO.transform.parent = voidLandTextPanel.transform;
                textPanelTextGO.name = "Text";
                textPanelTextGO.transform.localPosition = new Vector3(0.04f, 0.74f, 0f);
                textPanelTextGO.transform.localRotation = Quaternion.Euler(0f, 270f, 90f);
                textPanelTextGO.transform.localScale = new Vector3(6.0414f, 3.7636f, 6.462f);
                TextMeshPro voidLandTextPanelTMP = textPanelTextGO.GetComponent<TextMeshPro>();
                voidLandTextPanelTMP.text = "VoidLand";
                buttonToSwaptoVoidLand = Calls.Create.NewButton();
                buttonToSwaptoVoidLand.name = "VoidLandButton";
                buttonToSwaptoVoidLand.transform.parent = voidLandParent.transform;
                buttonToSwaptoVoidLand.transform.position = new Vector3(7.67f, 1.7f, 10f);
                buttonToSwaptoVoidLand.transform.localRotation = Quaternion.Euler(0f, 302.5f, 90f);
                buttonToSwaptoVoidLand.transform.GetChild(0).gameObject.GetComponent<InteractionButton>().onPressed.AddListener(new System.Action(() =>
                {
                    MelonCoroutines.Start(ToVoidLandPressed());
                }));
                voidLandParent.transform.position = new Vector3(0.72f, 0, -0.53f);
                gymInit = true;
            }
        }

        private IEnumerator ToVoidLandPressed()
        {
            PlayerManager.instance.localPlayer.Controller.gameObject.GetComponent<PlayerResetSystem>().ResetPlayerController();
            yield return new WaitForSeconds(1);
            RenderSettings.fog = false;
            voidLandActive = true;
            DeloadGym();
            LoadVoidLand();
            yield break;
        }

        private IEnumerator ReLoadVoidLand()
        {
            PlayerManager.instance.localPlayer.Controller.gameObject.GetComponent<PlayerResetSystem>().ResetPlayerController();
            yield return new WaitForSeconds(1);
            GameObject.DestroyImmediate(cube);
            yield return 0;
            LoadVoidLand();
            yield break;
        }

        private void TurnOffAllExtraRootObjects()
        {
            //creates an object and puts it in DontDestroyOnLoad Scene
            GameObject temp1 = new GameObject();
            GameObject.DontDestroyOnLoad(temp1);
            //grab the DontDestroyOnLoad Scene
            Scene ddolScene = temp1.scene;
            //then destroy the created object
            GameObject.DestroyImmediate(temp1);
            //grab list of DontDestroyOnLoad Scene GameObjects (this is just the root objects [no Parent])
            GameObject[] ddolGameObjects = ddolScene.GetRootGameObjects();
            //grab list of the active scene's GameObjects (this is just the root objects [no Parent])
            GameObject[] SceneGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            //for every DontDestroyOnLoad GameObject
            foreach (GameObject temp in ddolGameObjects)
            {
                //if GameObject is active (turned on) and it's not in a list of GameObjects to not turn off
                if (temp.active && !dontDisableGameObject.Contains(temp.name))
                {
                    //turn GameObject off
                    temp.SetActive(false);
                    //add to a list of GameObjects that got turned off
                    DisabledDDOLGameObjects.Add(temp);
                }
            }
            //for every GameObject in the list of the Scene's GameObjects
            foreach (GameObject temp in SceneGameObjects)
            {
                //if GameObject is active (turned on) and it's not in a list of GameObjects to not turn off
                if (temp.active && !dontDisableGameObject.Contains(temp.name))
                {
                    //turn GameObject off
                    temp.SetActive(false);
                }
                else if (temp.name == "--------------SCENE--------------")
                {
                    //turn off each child except Lighting and Effects
                    temp.transform.GetChild(0).gameObject.SetActive(false);
                    temp.transform.GetChild(1).gameObject.SetActive(false);
                    temp.transform.GetChild(2).gameObject.SetActive(false);
                    temp.transform.GetChild(3).gameObject.SetActive(false);
                    //turn GameObjects off (only turns off some Lighting and Effects)
                    temp.transform.GetChild(4).GetChild(0).gameObject.SetActive(false);
                    temp.transform.GetChild(4).GetChild(3).gameObject.SetActive(false);
                }
            }
        }

        private void DeloadGym()
        {
            ResetStructures();
            GameObject.Destroy(Calls.GameObjects.Gym.Logic.Bounds.SceneBoundaryPlayer.GetGameObject());
            TurnOffAllExtraRootObjects();
        }

        private void ResetStructures()
        {
            PoolManager.instance.GetPool("Disc").Reset(true);
            PoolManager.instance.GetPool("Ball").Reset(true);
            PoolManager.instance.GetPool("Pillar").Reset(true);
            PoolManager.instance.GetPool("RockCube").Reset(true);
            PoolManager.instance.GetPool("Wall").Reset(true);
            PoolManager.instance.GetPool("BoulderBall").Reset(true);
            PoolManager.instance.GetPool("SmallRock").Reset(true);
            PoolManager.instance.GetPool("LargeRock").Reset(true);
        }

        private void ReLoadGym()
        {
            Il2CppRUMBLE.Managers.SceneManager.instance.LoadSceneAsync(1, false, false, 2, LoadSceneMode.Single);
            RenderSettings.fog = true;
        }

        private void ReactivateDDOLObjects()
        {
            foreach (GameObject temp in DisabledDDOLGameObjects)
            {
                temp.SetActive(true);
            }
            DisabledDDOLGameObjects.Clear();
        }

        private void LoadVoidLand()
        {
            cube = new GameObject();
            cube.name = "Void Box";
            float size = (float)this.size;
            plane = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plane.name = "VoidFloor";
            plane.transform.parent = cube.transform;
            plane.transform.localScale = new Vector3(size, 0.01f, size);
            plane.transform.position = new Vector3(0, 0, 0);
            plane.layer = 9;
            MeshCollider meshCollider = plane.AddComponent<MeshCollider>();
            GroundCollider groundCollider = plane.AddComponent<GroundCollider>();
            Component.Destroy(plane.GetComponent<BoxCollider>());
            groundCollider.isMainGroundCollider = true;
            groundCollider.collider = meshCollider;
            plane.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
            plane.GetComponent<Renderer>().material.color = new Color(r, g, b);
            GameObject plane2 = GameObject.Instantiate(plane);
            plane2.transform.parent = cube.transform;
            plane2.transform.position = new Vector3(size / 2, size / 2, 0);
            plane2.transform.rotation = Quaternion.Euler(0, 0, 90);
            plane2.GetComponent<Renderer>().material.color = new Color(r, g, b);
            GameObject plane3 = GameObject.Instantiate(plane2);
            plane3.transform.parent = cube.transform;
            plane3.transform.position = new Vector3(-size / 2, size / 2, 0);
            plane3.transform.rotation = Quaternion.Euler(0, 0, 90);
            plane3.GetComponent<Renderer>().material.color = new Color(r, g, b);
            GameObject plane4 = GameObject.Instantiate(plane2);
            plane4.transform.parent = cube.transform;
            plane4.transform.position = new Vector3(0, size / 2, size / 2);
            plane4.transform.rotation = Quaternion.Euler(90, 0, 0);
            plane4.GetComponent<Renderer>().material.color = new Color(r, g, b);
            GameObject plane5 = GameObject.Instantiate(plane2);
            plane5.transform.parent = cube.transform;
            plane5.transform.position = new Vector3(0, size / 2, -size / 2);
            plane5.transform.rotation = Quaternion.Euler(90, 0, 0);
            plane5.GetComponent<Renderer>().material.color = new Color(r, g, b);
            GameObject plane6 = GameObject.Instantiate(plane2);
            plane6.transform.parent = cube.transform;
            plane6.transform.position = new Vector3(0, size, 0);
            plane6.transform.rotation = Quaternion.Euler(0, 0, 0);
            plane6.GetComponent<Renderer>().material.color = new Color(r, g, b);
        }
    }
}
