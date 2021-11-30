using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InTownUnitManager : Singletone<InTownUnitManager>
{
    [SerializeField] Transform trans;
    [SerializeField] CheckUnitSlot checkUnit;

    float timer;
    bool useTimer;
    unitStruct lastUnit;
    List<unitStruct> unitList;


    struct unitStruct
    {
        public CheckUnitSlot slot;
        public Unit.Unit_TYPE type;
        public Text countText;
    }

    private void Start()
    {
        unitList = new List<unitStruct>();

        lastUnit.type = Unit.Unit_TYPE.None;
        timer = 0f;
        useTimer = false;
    }

    private void Update()
    {
        if (unitList.Count <= 0)
            return;

        if (!useTimer)
        {
            timer = 0f;
            StartCoroutine(createTime(unitList[0]));
        }
    }

    public void OnCreatedUnit(Text unitCount, Text priceText, Unit.Unit_TYPE type)
    {
        //리스트에 추가
        unitStruct unit = new unitStruct();
        unit.type = type;
        unit.countText = unitCount;

        if (lastUnit.type != type) {
            CheckUnitSlot slot = Instantiate(checkUnit);
            slot.transform.parent = trans;
            slot.unitImage.sprite = Resources.Load<Sprite>($"UnitSprite/{type}");
            slot.unitCount.text = "1";
            unit.slot = slot;

            lastUnit = unit;
        }
        else
        {
            int num = int.Parse(lastUnit.slot.unitCount.text);
            lastUnit.slot.unitCount.text = (num+1).ToString();
            unit.slot = lastUnit.slot;
        }
        
        unitList.Add(unit);

        MyResourceData.Instance.UseJellyToMine(int.Parse(priceText.text));
    }

    IEnumerator createTime(unitStruct unit)
    {
        useTimer = true;
        while (true)
        {
            timer += Time.deltaTime;
            unit.slot.fillBox.fillAmount = timer / 3f;

            if (timer > 3f)
            {
                UnitManager.Instance.unitData[(int)unit.type].countUnit += 1;
                unit.countText.text = string.Format("{0:#,##0}", UnitManager.Instance.unitData[(int)unit.type].countUnit);
                Destroy(unit.slot.gameObject);

                unitList.RemoveAt(0);
                useTimer = false;
                break;
            }
            yield return null;
        }
    }
}
