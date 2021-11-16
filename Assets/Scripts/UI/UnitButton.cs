using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitButton : MonoBehaviour
{
    [SerializeField] Image unitImage;
    [SerializeField] Text priceText;

    Unit.Unit_TYPE type;
    Image backGround;


    public void Setup(UnitData unitData)
    {
        backGround = gameObject.GetComponent<Image>();

        // 데이터를 받아와 열거형으로 파싱.
        type = (Unit.Unit_TYPE)System.Enum.Parse(typeof(Unit.Unit_TYPE), unitData.GetData(Unit.KEY_TYPE));
        unitImage.sprite = Resources.Load<Sprite>($"UnitSprite/{type}");
        priceText.text = string.Format("{0:#,##0}", UnitManager.Instance.unitCount[type]);

        // 버튼에 이벤트 등록.
        GetComponent<Button>().onClick.AddListener(() => UnitManager.Instance.OnSelectedUnit(type));
    }

    public void OnCountUnit(UnitButton unitButton)
    {
        unitButton.priceText.text = $"{UnitManager.Instance.unitCount[type]}";
        if (UnitManager.Instance.unitCount[type] <= 0)
        {
            backGround.color = new Color(100f/255f, 100f / 255f, 100f / 255f);
            unitImage.color = new Color(100f / 255f, 100f / 255f, 100f / 255f);
        }
    }
}
