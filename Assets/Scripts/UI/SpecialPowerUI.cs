﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialPowerUI : MonoBehaviour {
    private Player _player;
    private GameObject _activeObject;
    private bool _active = false;
    private Lean.Touch.LeanFinger _finger;
    private Text _text;

    private Vector2 _velocity;
    private Vector2 _curPos;
    private Vector2 _lastPos;

    private float _speed = 10f;

    private TileTypes _type;
    public TileTypes.ESubState Type { get { return _type.Type;  }}

    private bool _readyForUse = false;
    private float _wiggleDirection = 1f;
    private float _wiggleSpeed = .008f;
    private float _wiggleThreshhold = .1f;

    private void Awake()
    {
        _type = new TileTypes();

        GetComponent<Image>().sprite = _type.Sprite;
        _text = transform.Find("Power").GetComponent<Text>();
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
        if (_active && _activeObject && _finger != null)
        {
            _activeObject.transform.position = _finger.GetWorldPosition(1f, Camera.current);
            _velocity = new Vector2 (_curPos.x - _lastPos.x, _curPos.y - _lastPos.y);
            _lastPos = _curPos;
            _curPos = _finger.GetWorldPosition(1f, Camera.current);
        }
    }

    public void UpdateText (float power)
    {
        _text.text = power + "/" + _player.settings.GetFillRequirementByType(_type.Type);
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

    public void SetColorType (TileTypes.ESubState state, Player myPlayer)
    {
        _type.Type = state;
        GetComponent<Image>().sprite = _type.Sprite;
        _player = myPlayer;

        if (myPlayer.selectedType.Type == state)
        {
            GetComponent<Image>().sprite = _type.SpecialitySprite;
        }
    }

    public void Fly ()
    {
        _active = false;
        _player.EmptyPower(_type.Type);

        if (_activeObject) { 
            Rigidbody2D rb = _activeObject.GetComponent<Rigidbody2D>();
            rb.velocity = _velocity * _speed;
            RootController.Instance.DisableControls();
        }
    }

    public void SetActive (Lean.Touch.LeanFinger finger, Player curPlayer)
    {
        if (_activeObject != null)
            Destroy(_activeObject);
        _activeObject = null;

        _active = true;
        _finger = finger;
        if (_type.Type == TileTypes.ESubState.red)
            _activeObject = Instantiate(Resources.Load<GameObject>("SpecialSelect"));
        else if (_type.Type == TileTypes.ESubState.yellow)
            _activeObject = Instantiate(Resources.Load<GameObject>("YellowSpecial"));
        else if (_type.Type == TileTypes.ESubState.blue)
            curPlayer.BlueTileEffect();
        else if (_type.Type == TileTypes.ESubState.green)
            curPlayer.GreenTileEffect();

        if (_activeObject != null)
        {
            _activeObject.name = "SpecialSelect";
            _activeObject.transform.SetParent(this.transform.parent.parent, false);
            _activeObject.GetComponent<MissileUI>().target = RootController.Instance.NextPlayer(curPlayer.playerNumber);
            _activeObject.GetComponent<MissileUI>().Type = _type.Type;

            _curPos = _finger.GetWorldPosition(1f, Camera.current);
            _lastPos = _finger.GetWorldPosition(1f, Camera.current);
        }
    }
}
