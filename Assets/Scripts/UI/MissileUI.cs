using UnityEngine;
using System.Collections;

public class MissileUI : MonoBehaviour
{
    private Player _target;
    public Player target { set { _target = value;  } }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == _target.playerString)
        {
            _target.ReceiveDamage(Mathf.RoundToInt(Constants.SpecialMoveFillRequirement));
            _target.SpecialExplosion();
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        Destroy(gameObject, 3f);
    }

    private void OnDestroy()
    {
        RootController.Instance.EnableControls();
    }
}