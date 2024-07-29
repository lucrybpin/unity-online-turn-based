using UnityEngine;

public class InputController
{
    [SerializeField] private ClientController _client;

    public InputController(ClientController clientController)
    {
        _client = clientController;
    }

    public void Update(Camera currentCamera)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Client - InputController - Clicked");
            Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit [] hits = Physics.RaycastAll(ray);
            foreach(RaycastHit hit in hits)
            {
                if(hit.transform.TryGetComponent<StartingCell>(out StartingCell cell))
                {
                    _client.AskForReposition(hit.transform.position);
                }
            }
        }
    }
}
