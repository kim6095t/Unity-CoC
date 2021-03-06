using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitButton : MonoBehaviour
{
    [SerializeField] Image unitImage;
    [SerializeField] public Text unitCount;
    [SerializeField] Image JellyImage;
    [SerializeField] public Text priceText;

    Unit.Unit_TYPE type;
    Image backGround;


    public void SetUpToWar(UnitData unitData)
    {
        backGround = gameObject.GetComponent<Image>();

        // 데이터를 받아와 열거형으로 파싱.
        type = (Unit.Unit_TYPE)System.Enum.Parse(typeof(Unit.Unit_TYPE), unitData.GetData(Unit.KEY_TYPE));
        unitImage.sprite = Resources.Load<Sprite>($"UnitSprite/{type}");
        unitCount.text = string.Format("{0:#,##0}", UnitManager.Instance.unitData[(int)type].countUnit);

        // 버튼에 이벤트 등록.
        GetComponent<Button>().onClick.AddListener(() => UnitCreateManager.Instance.OnSelectedUnit(type));
    }


    public void SetUpToTown(UnitData unitData)
    {
        backGround = gameObject.GetComponent<Image>();

        // 데이터를 받아와 열거형으로 파싱.
        type = (Unit.Unit_TYPE)System.Enum.Parse(typeof(Unit.Unit_TYPE), unitData.GetData(Unit.KEY_TYPE));
        unitImage.sprite = Resources.Load<Sprite>($"UnitSprite/{type}");
        unitCount.text = string.Format("{0:#,##0}", UnitManager.Instance.unitData[(int)type].countUnit);

        JellyImage.gameObject.SetActive(true);
        priceText.gameObject.SetActive(true);
        priceText.text = unitData.GetData(Unit.KEY_PRICE);


        GetComponent<Button>().onClick.AddListener(() => InTownUnitManager.Instance.OnCreatedUnit(unitCount, priceText, type));
    }

    public void OnCountUnit(UnitButton unitButton)
    {
        unitButton.unitCount.text = $"{UnitManager.Instance.unitData[(int)type].countUnit}";
        if (UnitManager.Instance.unitData[(int)type].countUnit <= 0)
        {
            backGround.color = new Color(100f/255f, 100f/255f, 100f/255f);
            unitImage.color = new Color(100f/255f, 100f/255f, 100f/255f);
        }
    }
}
