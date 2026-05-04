using UnityEngine;

public class ManagerPlaybackDummy : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            AudioManager.Instance.ToggleMusicPlayback();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            AudioManager.Instance.PlaySfx("Hits");
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            AudioManager.Instance.PlaySfx("Jumps");
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            AudioManager.Instance.PlaySfx("Pickups");
        }
    }
}
