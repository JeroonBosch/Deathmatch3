using UnityEngine;
using UnityEngine.UI;

public class Player : ScriptableObject
{
    public int playerNumber;
    public string playerString;

    private float _health;
    public float health { get { return _health; } set { _health = value;} }

    public Transform transform;
    private PortraitUI portrait;
    private Sprite _portraitSprite;

    private TileTypes _type1;
    public TileTypes type1 { get { return _type1; } }
    private float type1Power;

    private TileTypes _type2;
    public TileTypes type2 { get { return _type2; } }
    private float type2Power;

    private int _turn;
    public int turn { get { return _turn; } set { _turn = value; } }

    private bool isExploding = false;
    public bool exploding { set { isExploding = value; } }

    public void Init(string name, int number)
    {
        playerString = name;
        playerNumber = number;

        portrait = null;
        _health = Constants.PlayerStartHP;
        _type1 = new TileTypes();
        _type2 = new TileTypes();
        _type1.Type = TileTypes.ESubState.yellow;
        _type2.Type = TileTypes.ESubState.blue;
        turn = 0;
        type1Power = 0;
        type2Power = 0;
    }

    public void ReceiveDamage(int damage)
    {
        health -= damage;

        portrait.SetHitpoints(health, Constants.PlayerStartHP);

        if (health <= 0f)
        {
            RootController.Instance.TriggerEndScreen(this);
        }
    }

    public void Heal (int heal)
    {
        health += heal;
        health = Mathf.Min(Constants.PlayerStartHP, health);

        portrait.SetHitpoints(health, Constants.PlayerStartHP);
    }

    public void SelectColorByIndex(int index)
    {
        if ((TileTypes.ESubState.yellow + index) != _type1.Type && (TileTypes.ESubState.yellow + index) != _type2.Type)
        {
            _type1.Type = _type2.Type;
            _type2.Type = TileTypes.ESubState.yellow + index;
        }
    }

    public void SetUI (Transform transform)
    {
        this.transform = transform;
        if (transform.Find("PortraitHP"))
        {
            this.portrait = transform.Find("PortraitHP").GetComponent<PortraitUI>();
        }

        transform.Find("Color1").Find("Power").GetComponent<Text>().text = type1Power + "/" + Constants.SpecialMoveFillRequirement;
        transform.Find("Color2").Find("Power").GetComponent<Text>().text = type2Power + "/" + Constants.SpecialMoveFillRequirement;
    }

    public void SetTimerActive (bool active)
    {
        this.portrait.SetTimerActive(active);

        if (active)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            portrait.gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

            if (CheckPowerLevel_1())
                transform.Find("Color1").GetComponent<SpecialPowerUI>().SetReady();
            if (CheckPowerLevel_2())
                transform.Find("Color2").GetComponent<SpecialPowerUI>().SetReady();
        }
        else
        {
            transform.localScale = new Vector3(.7f, .7f, .7f);
            portrait.gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, .8f);

            transform.Find("Color1").GetComponent<SpecialPowerUI>().SetNotReady();
            transform.Find("Color2").GetComponent<SpecialPowerUI>().SetNotReady();
        }  
    }

    public void SetTimer (float time)
    {
        this.portrait.SetTimer(time);
    }

    public void FillPower (TileTypes.ESubState type, int comboSize)
    {
        if (type1.Type == type)
        {
            type1Power += (comboSize * Constants.SpecialMoveMultiplier);
            type1Power = Mathf.Min(Constants.SpecialMoveFillRequirement, type1Power);
        }

        if (type2.Type == type)
        {
            type2Power += (comboSize * Constants.SpecialMoveMultiplier);
            type2Power = Mathf.Min(Constants.SpecialMoveFillRequirement, type2Power);
        }

        transform.Find("Color1").Find("Power").GetComponent<Text>().text = type1Power + "/" + Constants.SpecialMoveFillRequirement;
        transform.Find("Color2").Find("Power").GetComponent<Text>().text = type2Power + "/" + Constants.SpecialMoveFillRequirement;

        if (CheckPowerLevel_1())
            transform.Find("Color1").GetComponent<SpecialPowerUI>().SetReady();
        if (CheckPowerLevel_2())
            transform.Find("Color2").GetComponent<SpecialPowerUI>().SetReady();
    }

    public void EmptyPower (TileTypes.ESubState type)
    {
        if (type1.Type == type)
            type1Power = 0;
        if (type2.Type == type)
            type2Power = 0;

        transform.Find("Color1").Find("Power").GetComponent<Text>().text = type1Power + "/" + Constants.SpecialMoveFillRequirement;
        transform.Find("Color2").Find("Power").GetComponent<Text>().text = type2Power + "/" + Constants.SpecialMoveFillRequirement;

        transform.Find("Color1").GetComponent<SpecialPowerUI>().SetNotReady();
        transform.Find("Color2").GetComponent<SpecialPowerUI>().SetNotReady();
    }

    public bool CheckPowerLevel_1 ()
    {
        if (type1Power >= Constants.SpecialMoveFillRequirement)
            return true;

        return false;
    }

    public bool CheckPowerLevel_2()
    {
        if (type2Power >= Constants.SpecialMoveFillRequirement)
            return true;

        return false;
    }

    public void SetPortraitSprite()
    {
        _portraitSprite = portrait.GetPortraitSprite();
    }

    public Sprite GetPortraitSprite ()
    {
        return _portraitSprite;
    }

    public void SpecialExplosion()
    {
        if (transform)
        {
            GameObject explosion = Instantiate(Resources.Load<GameObject>("SpecialExplosion"));
            explosion.transform.SetParent(transform.parent);
            explosion.transform.position = transform.position;

            Destroy(explosion, .94f);
        }
    }

    public void NormalExplosion()
    {
        if (transform && !isExploding)
        {
            isExploding = true;

            GameObject explosion = Instantiate(Resources.Load<GameObject>("NormalExplosion"));
            explosion.transform.SetParent(transform.parent);
            explosion.transform.position = transform.position;

            Destroy(explosion, .94f);
        }
    }
}