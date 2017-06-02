using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPowerUI : MonoBehaviour {

    private GameObject _activeObject;
    private bool _active = false;
    private Lean.Touch.LeanFinger _finger;

    private Vector2 _velocity;
    private Vector2 _curPos;
    private Vector2 _lastPos;

    private float _speed = 10f;

    private TileTypes _type;

    private bool _readyForUse = false;
    private float _wiggleDirection = 1f;
    private float _wiggleSpeed = .008f;
    private float _wiggleThreshhold = .1f;

    private void Awake()
    {
        _type = new TileTypes();
    }

    void Update()
    {
        if (_readyForUse)
        {
            RectTransform rt = GetComponent<RectTransform>();
            if (Mathf.Abs(rt.localRotation.z) >= _wiggleThreshhold)
                _wiggleDirection = -1f * _wiggleDirection;

            float z = rt.localRotation.z + _wiggleSpeed * _wiggleDirection;
            rt.localRotation = new Quaternion(rt.localRotation.x, rt.localRotation.y, z, rt.localRotation.w);
        }
    }

    void LateUpdate () {
        if (_active && _finger != null)
        {
            _activeObject.transform.position = _finger.GetWorldPosition(1f, Camera.current);
            _velocity = new Vector2 (_curPos.x - _lastPos.x, _curPos.y - _lastPos.y);
            _lastPos = _curPos;
            _curPos = _finger.GetWorldPosition(1f, Camera.current);
        }
    }

    public void SetReady ()
    {
        _readyForUse = true;
        transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }

    public void SetNotReady ()
    {
        _readyForUse = false;
        transform.localScale = new Vector3(1f, 1f, 1f);
        transform.localRotation = new Quaternion(0f, 0f, 0f, transform.localRotation.w);
    }

    public void SetColorType (TileTypes.ESubState state)
    {
        _type.Type = state;
    }

    public void Fly (Player curPlayer)
    {
        _active = false;
        Rigidbody2D rb = _activeObject.GetComponent<Rigidbody2D>();
        rb.velocity = _velocity * _speed;

        curPlayer.EmptyPower(_type.Type);

        RootController.Instance.DisableControls();
    }

    public void SetActive (Lean.Touch.LeanFinger finger, Player curPlayer)
    {
        if (_activeObject != null)
            Destroy(_activeObject);

        _active = true;
        _finger = finger;
        _activeObject = Instantiate(Resources.Load<GameObject>("SpecialSelect"));
        _activeObject.name = "SpecialSelect";
        _activeObject.transform.SetParent(this.transform.parent.parent, false);
        _activeObject.GetComponent<MissileUI>().target = RootController.Instance.NextPlayer(curPlayer.playerNumber);


        _curPos = _finger.GetWorldPosition(1f, Camera.current);
        _lastPos = _finger.GetWorldPosition(1f, Camera.current);
    }
}
