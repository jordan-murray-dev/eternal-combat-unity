using System.Collections;
using UnityEngine;

namespace EC.Gameplay
{
    public class ProcedureGameplay : ProcedureFsmState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            UnityEngine.SceneManagement.SceneManager.LoadScene("GamePlay");

            GameLog.Debug("Enter ProcedureGameplay.");

            EC.App.MonoManager.StartCoroutine(OnEnterCoroutine());
        }

        private IEnumerator OnEnterCoroutine()
        {
            yield return null;

            GameObject player1, player2;

            GameObject player1Prefab = (GameObject)App.ResourceManager.LoadAsset("Assets/Res/Prefabs/Character/Player1.prefab");
            GameObject player2Prefab = (GameObject)App.ResourceManager.LoadAsset("Assets/Res/Prefabs/Character/Player2.prefab");

            player1 = GameObject.Instantiate(player1Prefab, GameObject.Find("SpawnPoints/Player1").transform.position, Quaternion.identity);
            player2 = GameObject.Instantiate(player2Prefab, GameObject.Find("SpawnPoints/Player2").transform.position, Quaternion.identity);

            player1.tag = "player1";
            player2.tag = "player2";

            // Initialize the roundmanager
            UnityEngine.GameObject.FindFirstObjectByType<RoundManager>().Initialize(player1, player2);
        }
    }
}