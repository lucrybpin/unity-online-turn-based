using System.Collections.Generic;
using UnityEngine;

public struct InputResult
{
    public bool LeftClick;
    public bool RightClick;
    public Vector3 WorldPosition;
    public GameObject FirstObject;
    public List<GameObject> Objects;
}

public class InputController
{

    private InputResult _inputResult;

    [SerializeField] private ClientController _client;

    public InputController(ClientController clientController)
    {
        _inputResult = new InputResult();
        _client = clientController;
        _inputResult.Objects = new List<GameObject>();
    }

    public InputResult Update(Camera currentCamera)
    {
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        _inputResult.LeftClick = false;
        _inputResult.RightClick = false;
        _inputResult.FirstObject = null;
        _inputResult.Objects.Clear();
        if (Input.GetMouseButtonDown(0))
            _inputResult.LeftClick = true;
        if (Input.GetMouseButtonDown(1))
            _inputResult.RightClick = true;

        for (int i = 0; i < hits.Length; i++)
        {
            if(i == 0)
            {
                _inputResult.FirstObject = hits[i].transform.gameObject;
                _inputResult.WorldPosition = hits[i].point;
            }
            _inputResult.Objects.Add(hits[i].transform.gameObject);
        }

        return _inputResult;
    }
}
