using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//using System;

[System.Serializable]
public class SpellSlot
    {
        public Spell spell;
        public int level = 1;

    public SpellSlot(Spell spell)
        {
            this.spell = spell;
            this.level = level;
    }
    }






public class SpellMaster : MonoBehaviour
{

    [SerializeField] private int size = 8;

    [SerializeField] public UnityEvent OnSpellsChanged;
    

    [SerializeField] public List<SpellSlot> spells = new List<SpellSlot>();

    public int targetSpell;

    public Text descriptionSpellGameMenu;

    PlayerController playerController;
    playerstat playerstat;
    Animator anim;
    PassiveMaster passiveMaster;
    public BuffsMaster buffsMaster;
    

    public LineRenderer LineRend1;

    public Material materialLineRenderer;
    public RectTransform pointer;
    bool pointerSpell;
    Vector3 pointerTarget;
    int touchNumber;


    //[SerializeField] private List<Image> buffsIcons = new List<Image>();
    [SerializeField] private float[] cdList = new float[200];
    [SerializeField] private Text[] cdText = new Text[10];
    [SerializeField] private float[] cdFloat = new float[10];
    [SerializeField] private Image[] cdImage = new Image[10];

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerstat = GetComponent<playerstat>();
        anim = GetComponentInChildren<Animator>();
        passiveMaster = GetComponent<PassiveMaster>();
        if (Manager.DoorCount >= 1)
        {
            OnSpellsChanged.Invoke();
        }


        //LineRend1 = gameObject.AddComponent<LineRenderer>();
        LineRend1 = GetComponent<LineRenderer>();
        LineRend1.enabled = false;
    }

    void Update()
    {

        for (int i = 0; i < 199; i++)
        {
            if (cdList[i] > 0) cdList[i] -= Time.deltaTime;
        }
        for (int i = 0; i < 8; i++)
        {
            if (cdFloat[i] > 0) { cdFloat[i] -= Time.deltaTime; cdText[i].text = "" + cdFloat[i]; cdImage[i].GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f); }

            if (cdFloat[i] <= 0) { cdText[i].text = ""; cdImage[i].GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.0f); }
        }
        // FADE ICON IF NOT ENOUGH MANA
        if (spells != null)
        {
            for (int i = 0; i < spells.Count; i++)
            { if (spells[i].spell.mana > playerstat.curMP) cdImage[i].GetComponent<Image>().color = new Color(0f, 0f, 0.5f, 0.5f); }
        }





        if (LineRend1.enabled == true && playerstat.states[0] <= 0)
        {
            LineRend1.SetPosition(0, new Vector3(transform.position.x, 1, transform.position.z));
            LineRend1.SetPosition(1, transform.TransformPoint(Vector3.forward * 9 + Vector3.up * 1));
        }
        else { LineRend1.enabled = false; }

        if (pointerSpell == true)

        {
            //if (Input.touchCount > 0)

            Touch touch = Input.GetTouch(touchNumber);
            pointer.position = touch.position;


            //if (Input.touchCount == 1)
            //{
            //    Touch touch = Input.GetTouch(0);
            //    pointer.position = touch.position;
            //}
            //if (Input.touchCount == 2)
            //{
            //    Touch touch = Input.GetTouch(1);
            //    pointer.position = touch.position;
            //}

            //////////////////////////


            //Vector2 pos = Input.mousePosition;
            //pointer.position = pos;
        }


        /////////////////////////


        //if (Input.touchCount > 0)
        //{
        //    Touch touch = Input.GetTouch(0);

        //    // Update the Text on the screen depending on current position of the touch each frame
        //    m_Text.text = "Touch Position : " + touch.position;
        //}
        //else
        //{
        //    m_Text.text = "No touch contacts";
        //}

    }

    public void StartCasting()
    {

        LineRend1.enabled = true;
        LineRend1.material = materialLineRenderer;

    }

    public bool AddSpell(Spell spell)
    {
        SpellSlot new_slot = new SpellSlot(spell);

        foreach (SpellSlot slot in spells)
        {
            if (slot.spell.id == spell.id)
            {
                slot.level += 1;
                OnSpellsChanged.Invoke();
                return false;
            }
        }

        if (spells.Count >= 8) return false;
        spells.Add(new_slot);

        OnSpellsChanged.Invoke();
        return true;
    }

    public Spell GetItem(int i) { return i < spells.Count ? spells[i].spell : null; }

    //public int GetAmount(int i) { return i < items.Count ? items[i].amount : 0; }

    public int GetSize() { return spells.Count;  }

    //public void EverySec()
    //{
    //    for (int i = 0; i < 199; i++)
    //    {
    //        if (cdList[i] > 0) cdList[i] -= 1f;
    //    }
    //    for (int i = 0; i < 8; i++)
    //    {
    //        if (cdFloat[i] >= 1) { cdFloat[i] -= 1f; cdText[i].text = "" + cdFloat[i]; cdImage[i].GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f); }
    //        if (cdFloat[i] == 0) { cdText[i].text = ""; cdImage[i].GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.0f); }
    //    }
    //    //if (cdFloat[0] > 0) cdFloat[0] -= 1f; cdText[0].text = "" + cdFloat[0];
    //    //icons[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
    //}

    public void ButtonDown(int number)

    {
        if (cdList[spells[number].spell.id] <= 0 && playerstat.states[1] <= 0 && playerstat.curMP >= spells[number].spell.mana)
        {
            if (spells[number].spell.casting == true) StartCasting();
            if (spells[number].spell.pointTarget == true)
            {
                pointerSpell = true;
                pointer.localScale = new Vector3(1, 1);
                if (Input.touchCount == 1) touchNumber = 0;
                if (Input.touchCount == 2) touchNumber = 1;
                if (Input.touchCount == 3) touchNumber = 2;
            }
            Debug.Log("BUTTON DOWNNNN 2222222");
        }
    }

    public void ButtonUp(int number)
    {
        if (cdList[spells[number].spell.id] <= 0 && playerstat.states[1] <= 0 && playerstat.curMP >= spells[number].spell.mana)
        {
            Ray ray = Camera.main.ScreenPointToRay(pointer.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                //Debug.DrawLine(ray.origin, hit.point);
                // Debug.Log(hit.point);
                pointerTarget = hit.point;
            }

            Debug.Log("BUTTON UPPPPPP 2222222");



            pointerSpell = false;

            pointer.localScale = new Vector3(0, 0);
            LineRend1.enabled = false;

            cdFloat[number] = spells[number].spell.cooldown;
            CastSpell(spells[number].level, spells[number].spell);
        }
    }


    //public void SpellButton00() { if (cdList[spells[0].spell.id] <= 0 && playerstat.states[1] <= 0 && playerstat.curMP >= spells[0].spell.mana) { cdFloat[0] = spells[0].spell.cooldown; CastSpell(spells[0].level, spells[0].spell); } }
    //public void SpellButton01() { if (cdList[spells[1].spell.id] <= 0 && playerstat.states[1] <= 0 && playerstat.curMP >= spells[1].spell.mana) { cdFloat[1] = spells[1].spell.cooldown; CastSpell(spells[1].level, spells[1].spell); } }
    //public void SpellButton02() { if (cdList[spells[2].spell.id] <= 0 && playerstat.states[1] <= 0 && playerstat.curMP >= spells[2].spell.mana) { cdFloat[2] = spells[2].spell.cooldown; CastSpell(spells[2].level, spells[2].spell); } }
    //public void SpellButton03() { if (cdList[spells[3].spell.id] <= 0 && playerstat.states[1] <= 0 && playerstat.curMP >= spells[3].spell.mana) { cdFloat[3] = spells[3].spell.cooldown; CastSpell(spells[3].level, spells[3].spell); } }
    //public void SpellButton04() { if (cdList[spells[4].spell.id] <= 0 && playerstat.states[1] <= 0 && playerstat.curMP >= spells[4].spell.mana) { cdFloat[4] = spells[4].spell.cooldown; CastSpell(spells[4].level, spells[4].spell); } }

    //public void SpellButton05() { if (cdList[spells[5].spell.id] <= 0 && playerstat.states[1] <= 0 && playerstat.curMP >= spells[5].spell.mana) { cdFloat[5] = spells[5].spell.cooldown; CastSpell(spells[5].level, spells[5].spell); } }
    //public void SpellButton06() { if (cdList[spells[6].spell.id] <= 0 && playerstat.states[1] <= 0 && playerstat.curMP >= spells[6].spell.mana) { cdFloat[6] = spells[6].spell.cooldown; CastSpell(spells[6].level, spells[6].spell); } }
    //public void SpellButton07() { if (cdList[spells[7].spell.id] <= 0 && playerstat.states[1] <= 0 && playerstat.curMP >= spells[7].spell.mana) { cdFloat[7] = spells[7].spell.cooldown; CastSpell(spells[7].level, spells[7].spell); } }

    private void CastSpell(int level, Spell spell)

    {
        passiveMaster.CastSpellEvent();

        ////thunder strike 00
        //if (spell.id == 0)
        //{
        //    playerstat.ReduceMana(spell.mana);
        //    cdList[spell.id] = spell.cooldown;

        //    //Instantiate(masterEffect.effects[0], transform.position + Vector3.up * 0.5f, transform.rotation); // change effect root
        //    anim.SetTrigger("Stomp");

        //    int damage = 0;

        //    if (spell.isDamaging == true)
        //    {
        //        for (int i = 0; i < 50; i++)
        //        {
        //            if (spell.feature[i] > 0)
        //            {
        //                if (level == 1) damage += (spell.feature[i] * playerstat.stats.stats[200 + i]) / 100;
        //                if (level > 1) damage += (spell.feature[i] + (spell.forLevelFeature[i] * (level - 1)) * playerstat.stats.stats[200 + i]) / 100;


        //            }
        //        }
        //    }





        //    Collider[] _col = Physics.OverlapSphere(transform.position, 3);
        //    foreach (var col in _col)
        //    {
        //        if (col.gameObject.tag == "Enemy")
        //        {
        //            UnitController unitController = col.GetComponent<UnitController>();
        //            unitController.MakeDamage(damage, 0, 0, 0);
        //            if (spell.buff) unitController.AddBuff(spell.buff, level);
        //        }

        //    }
        //}
        ////thrust 01
        //if (spell.id == 1)
        //{

        //    playerstat.ReduceMana(spell.mana);
        //    cdList[spell.id] = spell.cooldown;
        //    Collider[] _col = Physics.OverlapSphere(transform.TransformPoint(Vector3.forward * 2), 3f);

        //    //Instantiate(masterEffect.effects[0], transform.TransformPoint(Vector3.forward * 2), transform.rotation);
        //    //Debug.Log("I CAST THE THRUST!!!!!! player");
        //    anim.SetTrigger("Stomp");
        //    foreach (var col in _col)
        //    {
        //        if (col.gameObject.tag == "Enemy")
        //        {
        //            UnitController unitController = col.GetComponent<UnitController>();
        //            unitController.TakeThrust(1);
        //            // col.gameObject.SendMessage("TakeThrust", 1, SendMessageOptions.DontRequireReceiver);
        //        }


        //    }
        //}
        //// immolation
        //if (spell.id == 2)
        //{
        //    anim.SetTrigger("HandUp");
        //    playerstat.ReduceMana(spell.mana);
        //    cdList[spell.id] = spell.cooldown;

        //    if (spell.isDamaging == true)
        //    {
        //        int damage = 0;

        //        for (int i = 0; i < 50; i++)
        //        {
        //            if (spell.feature[i] > 0) { damage += (spell.feature[i] * playerstat.stats.stats[200 + i]) / 100; }
        //        }
        //        Debug.Log("Damage immolation = " + damage);
        //    }

        //    playerstat.AddBuff(spell.buff, level);
        //}
        //// small heal 03
        //if (spell.id == 3 && cdList[spell.id] <= 0 && playerstat.curMP >= spell.mana)
        //{
        //    playerstat.ReduceMana(spell.mana);
        //    cdList[spell.id] = spell.cooldown;
        //    float heal = 100f;
        //    playerstat.DealHeal(heal);
        //    anim.SetTrigger("HandUp");
        //}
        ////rage 04
        //if (spell.id == 4 && cdList[spell.id] <= 0 && playerstat.curMP >= spell.mana)
        //{
        //    playerstat.ReduceMana(spell.mana);
        //    cdList[spell.id] = spell.cooldown;

        //    playerstat.AddBuff(spell.buff, level);
        //    anim.SetTrigger("HandUp");

        //}
        ////divine shield 05
        //if (spell.id == 5 && cdList[spell.id] <= 0 && playerstat.curMP >= spell.mana)
        //{
        //    playerstat.ReduceMana(spell.mana);
        //    cdList[spell.id] = spell.cooldown;

        //    playerstat.AddBuff(spell.buff, level);
        //    anim.SetTrigger("Roar");

        //}
        //if (spell.id == 6 && cdList[spell.id] <= 0 && playerstat.curMP >= spell.mana)
        //{
        //    playerstat.ReduceMana(spell.mana);
        //    cdList[spell.id] = spell.cooldown;

        //    playerstat.AddBuff(spell.buff, level);
        //    anim.SetTrigger("HandUp");

        //}
        //if (spell.id == 7 && cdList[spell.id] <= 0 && playerstat.curMP >= spell.mana)
        //{
        //    playerstat.ReduceMana(spell.mana);
        //    cdList[spell.id] = spell.cooldown;

        //    playerstat.AddBuff(spell.buff, level);
        //    anim.SetTrigger("HandUp");

        //}
        //if (spell.id == 8 && cdList[spell.id] <= 0 && playerstat.curMP >= spell.mana)
        //{
        //    playerstat.ReduceMana(spell.mana);
        //    cdList[spell.id] = spell.cooldown;

        //    playerstat.AddBuff(spell.buff, level);
        //    anim.SetTrigger("Roar");

        //}
        //// frost arrow
        //if (spell.id == 9 && cdList[spell.id] <= 0 && playerstat.curMP >= spell.mana)
        //{
        //    playerstat.ReduceMana(spell.mana);
        //    cdList[spell.id] = spell.cooldown;


        //    anim.SetTrigger("Cast00");

        //    GameObject newBullet;

        //    newBullet = Instantiate(spell.bullet);

        //    newBullet.transform.rotation = transform.rotation;
        //    newBullet.transform.position = transform.TransformPoint(Vector3.up * 1f);

        //    Bullet bulletController = newBullet.GetComponent<Bullet>();
        //    bulletController.ally = true;
        //    bulletController.speed = 10f;
        //    bulletController.damage = playerstat.stats.stats[224] * 2;
        //    bulletController.dest = transform.forward;
        //}

        ////thunder strike 10
        //if (spell.id == 10)
        //{
        //    playerstat.ReduceMana(spell.mana);
        //    cdList[spell.id] = spell.cooldown;

        //    Instantiate(spell.effect, transform.position + Vector3.up * 0.9f, transform.rotation); // change effect root
        //    anim.SetTrigger("Roar");

        //    int damage = 0;

        //    if (spell.isDamaging == true)
        //    {
        //        for (int i = 0; i < 50; i++)
        //        {
        //            if (spell.feature[i] > 0)
        //            {
        //                if (level == 1) damage += (spell.feature[i] * playerstat.stats.stats[200 + i]) / 100;
        //                if (level > 1) damage += (spell.feature[i] + (spell.forLevelFeature[i] * (level - 1)) * playerstat.stats.stats[200 + i]) / 100;


        //            }
        //        }
        //    }





        //    Collider[] _col = Physics.OverlapSphere(transform.position, 3);
        //    foreach (var col in _col)
        //    {
        //        if (col.gameObject.tag == "Enemy")
        //        {
        //            UnitController unitController = col.GetComponent<UnitController>();
        //            unitController.MakeDamage(damage, 0, 0, 0);
        //            if (spell.buff) unitController.AddBuff(spell.buff, level);
        //            if (spell.effectTarget) Instantiate(spell.effectTarget, col.transform.position + Vector3.up * 0.5f, transform.rotation);
        //        }

        //    }
        //}
        //// arcane  arrows 11
        //if (spell.id == 11 && cdList[spell.id] <= 0 && playerstat.curMP >= spell.mana)
        //{
        //    playerstat.ReduceMana(spell.mana);
        //    cdList[spell.id] = spell.cooldown;


        //    anim.SetTrigger("Cast00");

        //    for (int i = 0; i < 10; i++)
        //    {

        //        GameObject newBullet;

        //        newBullet = Instantiate(spell.bullet);

        //        newBullet.transform.rotation = transform.rotation;
        //        //newBullet.transform.position = transform.TransformPoint(Vector3.forward * i);
        //        newBullet.transform.position = transform.TransformPoint(Vector3.forward * 2);

        //        Bullet bulletController = newBullet.GetComponent<Bullet>();
        //        Animator bulletAnim = newBullet.GetComponent<Animator>();
        //        bulletController.ally = true;
        //        bulletController.speed = 7f;
        //        bulletController.damage = playerstat.stats.stats[224];
        //        bulletController.dest = transform.forward;
        //        bulletController.wait = true;
        //        bulletController.Wait(0.05f * i);

        //        int rand = Random.Range(1, 11);
        //        Debug.Log(rand);
        //        bulletAnim.SetInteger("animation", rand);
        //        //bulletAnim.SetInteger("animation", 3);
        //        //bulletAnim.SetInteger("animation", 33);
        //    }
        //}

        //// arcane  bolt
        //if (spell.id == 12 && cdList[spell.id] <= 0 && playerstat.curMP >= spell.mana)
        //{
        //    playerstat.ReduceMana(spell.mana);
        //    cdList[spell.id] = spell.cooldown;


        //    anim.SetTrigger("Cast00");

        //    GameObject newBullet;

        //    newBullet = Instantiate(spell.bullet);

        //    newBullet.transform.rotation = transform.rotation;
        //    newBullet.transform.position = transform.TransformPoint(Vector3.up * 1f);

        //    Bullet bulletController = newBullet.GetComponent<Bullet>();
        //    bulletController.ally = true;
        //    bulletController.speed = 10f;
        //    bulletController.damage = playerstat.stats.stats[224] * 7;
        //    bulletController.dest = transform.forward;
        //}



        //// FIRE  bolt
        //if (spell.id == 13 && cdList[spell.id] <= 0 && playerstat.curMP >= spell.mana)
        //{
        //    playerstat.ReduceMana(spell.mana);
        //    cdList[spell.id] = spell.cooldown;


        //    anim.SetTrigger("Cast00");

        //    GameObject newBullet;

        //    newBullet = Instantiate(spell.bullet);

        //    newBullet.transform.rotation = transform.rotation;
        //    newBullet.transform.position = transform.TransformPoint(Vector3.up * 1f);

        //    Bullet bulletController = newBullet.GetComponent<Bullet>();
        //    bulletController.ally = true;
        //    bulletController.speed = 10f;
        //    bulletController.damage = playerstat.stats.stats[224] * 7;
        //    bulletController.dest = transform.forward;
        //}

        //// Arcane  Explode
        //if (spell.id == 14 && cdList[spell.id] <= 0 && playerstat.curMP >= spell.mana)
        //{
        //    playerstat.ReduceMana(spell.mana);
        //    cdList[spell.id] = spell.cooldown;


        //    anim.SetTrigger("Cast00");

        //    Instantiate(spell.effect, pointerTarget + Vector3.up * 2f, transform.rotation); // change effect root

        //    Collider[] _col = Physics.OverlapSphere(pointerTarget, 3);
        //    foreach (var col in _col)
        //    {
        //        if (col.gameObject.tag == "Enemy")
        //        {
        //            UnitController unitController = col.GetComponent<UnitController>();
        //            unitController.MakeDamage(50, 0, 0, 0);
        //            if (spell.buff) unitController.AddBuff(spell.buff, level);
        //        }

        //    }
        //}





        //// shield block
        //if (spell.id == 12434552)
        //{
        //    playerstat.ReduceMana(spell.mana);
        //    cdList[spell.id] = spell.cooldown;

        //    playerstat.AddBuff(spell.buff, level);
        //    anim.SetTrigger(spell.animTrigger);

        //}




        if (spell.id >= 0 && spell.id < 299)
        {
            playerstat.ReduceMana(spell.mana);
            cdList[spell.id] = spell.cooldown;
            anim.SetTrigger(spell.animTrigger);

            if (spell.buff && spell.onMe == true) playerstat.AddBuff(spell.buff, level);


            // damage
            int damage = 0;
            if (spell.isDamaging == true)
            {

                for (int i = 0; i < 100; i++)
                {
                    if (spell.feature[i] > 0)
                    {
                        if (level == 1) damage += (spell.feature[i] * playerstat.stats.stats[400 + i]) / 100;
                        if (level > 1) damage += (spell.feature[i] + (spell.forLevelFeature[i] * (level - 1)) * playerstat.stats.stats[400 + i]) / 100;
                    }
                }
                if (spell.typeDamage == 2) damage = (damage * playerstat.stats.stats[454]) / 100; // fire
                if (spell.typeDamage == 3) damage = (damage * playerstat.stats.stats[455]) / 100; // ice
                if (spell.typeDamage == 4) damage = (damage * playerstat.stats.stats[456]) / 100; // electro
                if (spell.typeDamage == 5) damage = (damage * playerstat.stats.stats[457]) / 100; // nature
                if (spell.typeDamage == 6) damage = (damage * playerstat.stats.stats[458]) / 100; // poison
                if (spell.typeDamage == 7) damage = (damage * playerstat.stats.stats[459]) / 100; // dark arts
                if (spell.typeDamage == 8) damage = (damage * playerstat.stats.stats[460]) / 100; // light
                //if (spell.typeDamage == 10) damage = (damage * playerstat.stats.stats[454]) / 100; // bleeding



            }


            int damageBuff = 0;

            if (spell.buff)
            {
                if (spell.buff.isDamaging == true)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        if (spell.buff.feature[i] > 0)
                        {
                            if (level == 1) damageBuff += (spell.feature[i] * playerstat.stats.stats[400 + i]) / 100;
                            if (level > 1) damageBuff += (spell.feature[i] + (spell.forLevelFeature[i] * (level - 1)) * playerstat.stats.stats[400 + i]) / 100;
                        }
                    }
                }
            }
            // lifesteal

            if (spell.lifeSteal > 0) playerstat.DealHeal((spell.lifeSteal * damage) / 100);



            // healing

            int heal = 0;
            if (spell.isHealing == true)
            {

                for (int i = 0; i < 100; i++)
                {
                    if (spell.feature[i] > 0)
                    {
                        if (level == 1) heal += (spell.feature[i] * playerstat.stats.stats[400 + i]) / 100;
                        if (level > 1) heal += (spell.feature[i] + (spell.forLevelFeature[i] * (level - 1)) * playerstat.stats.stats[400 + i]) / 100;
                    }
                }
                playerstat.DealHeal(heal);
            }

            // collider action
            if (spell.radiusCollider > 0 && spell.forwardCollider == 0 && spell.delay == 0 && spell.pointTarget == false)
            {
                Collider[] _col = Physics.OverlapSphere(transform.position, spell.radiusCollider);
                if (spell.effect) Instantiate(spell.effect, transform.position + Vector3.up * 0.5f, transform.rotation);
                foreach (var col in _col)
                {
                    if (col.gameObject.tag == "Enemy")
                    {
                        UnitController unitController = col.GetComponent<UnitController>();
                        unitController.MakeDamage(damage, 0, 0, 0);
                        if (spell.buff) unitController.AddBuff(spell.buff, level, damageBuff);
                    }

                }


            }
            // collider action forward
            if (spell.radiusCollider > 0 && spell.forwardCollider > 0 && spell.delay == 0 && spell.pointTarget == false)
            {
                Collider[] _col = Physics.OverlapSphere(transform.TransformPoint(Vector3.forward * spell.forwardCollider), spell.radiusCollider);
                if (spell.effect) Instantiate(spell.effect, transform.TransformPoint(Vector3.forward * spell.forwardCollider), transform.rotation);
                foreach (var col in _col)
                {
                    if (col.gameObject.tag == "Enemy")
                    {
                        UnitController unitController = col.GetComponent<UnitController>();
                        unitController.MakeDamage(damage, 0, 0, 0);
                        if (spell.buff) unitController.AddBuff(spell.buff, level, damageBuff);
                    }

                }


            }

            // pointer action 
            if (spell.radiusCollider > 0 && spell.forwardCollider == 0 && spell.delay == 0 && spell.pointTarget == true)
            {
                Collider[] _col = Physics.OverlapSphere(new Vector3(pointerTarget.x, 1f, pointerTarget.z), spell.radiusCollider);
                if (spell.effect) Instantiate(spell.effect, new Vector3(pointerTarget.x, 1f, pointerTarget.z), transform.rotation);
                foreach (var col in _col)
                {
                    if (col.gameObject.tag == "Enemy")
                    {
                        UnitController unitController = col.GetComponent<UnitController>();
                        if (damage > 0) unitController.MakeDamage(damage, 0, 0, 0);
                        if (spell.buff) unitController.AddBuff(spell.buff, level, damageBuff);
                    }

                }


            }


            // effect forward
            if (spell.radiusCollider == 0 && spell.forwardCollider > 0 && spell.effect && spell.delay == 0) if (spell.effect) Instantiate(spell.effect, transform.position + Vector3.up * 0.5f, transform.rotation);
            // effect
            if (spell.radiusCollider == 0 && spell.forwardCollider == 0 && spell.effect && spell.delay == 0 && spell.bullet == null && spell.pointTarget == false) if (spell.effect) Instantiate(spell.effect, transform.position + Vector3.up * 0.6f, transform.rotation);
            // action with delay
            if (spell.radiusCollider > 0 && spell.delay > 0) StartCoroutine(DelayColliderDamageEffect(spell.delay, spell.radiusCollider, spell.forwardCollider, damage, damageBuff, spell.buff, spell.effect, level));



            // bullet
            if (spell.bullet && spell.pointTarget == false && spell.delay == 0)
            {
                for (int i = 0; i < spell.bulletCount; i++)
                {

                    GameObject newBullet;

                    newBullet = Instantiate(spell.bullet);

                    newBullet.transform.rotation = transform.rotation;
                    //newBullet.transform.position = transform.TransformPoint(Vector3.forward * i);
                    newBullet.transform.position = transform.TransformPoint(Vector3.forward * 2);

                    Bullet bulletController = newBullet.GetComponent<Bullet>();
                    Animator bulletAnim = newBullet.GetComponent<Animator>();
                    bulletController.ally = true;
                    bulletController.speed = 7f;
                    bulletController.damage = damage;
                    bulletController.damageType = spell.typeDamage;
                    bulletController.dest = transform.forward;
                    bulletController.wait = true;
                    bulletController.Wait(0.05f * i);
                    if (spell.bulletRandomTraectory == true)
                    {
                        int rand = Random.Range(1, 11);

                        bulletAnim.SetInteger("animation", rand);
                    }

                }
            }
            // effect area in point
            if (spell.effect && spell.pointTarget == true && spell.radiusCollider == 0)
            {
                GameObject smt = Instantiate(spell.effect, new Vector3(pointerTarget.x, 1f, pointerTarget.z), transform.rotation);
                SpellPoint spellPoint = smt.GetComponent<SpellPoint>();
                spellPoint.damage = damage;

            }
            // bullet with delay
            if (spell.bullet && spell.pointTarget == false && spell.delay > 0)
            {
                StartCoroutine(DelayBulletMaker(spell.delay, spell, level, damage));
            }


            // summon

            if (spell.isSummoning == true)
            {
                GameObject smt = Instantiate(spell.summon, transform.position + Vector3.forward * 2f, transform.rotation);
                Summon summon = smt.GetComponent<Summon>();
                summon.owner = gameObject;

            }


            // thrust
            if (spell.thrust == true)
            {
                Collider[] _col = Physics.OverlapSphere(transform.TransformPoint(Vector3.forward * spell.forwardCollider), spell.radiusCollider);
                //if (spell.effect) Instantiate(spell.effect, transform.TransformPoint(Vector3.forward * spell.forwardCollider), transform.rotation);
                foreach (var col in _col)
                {
                    if (col.gameObject.tag == "Enemy")
                    {
                        UnitController unitController = col.GetComponent<UnitController>();
                        unitController.player = gameObject;
                        unitController.TakeThrust(1);
                    }

                }

            }

            Debug.Log("Spell: " + spell.name + " damage: " + damage);


        }
        if (spell.id == 33) // jump
        {
            playerController.JumpStart();
            // Debug.Log("juuuump");
        }

        if (spell.id == 1) // thrust
        {

            Collider[] _col = Physics.OverlapSphere(transform.TransformPoint(Vector3.forward * 1.3f), 1.2f);


            foreach (var col in _col)
            {
                if (col.gameObject.tag == "Enemy")
                {
                    UnitController unitController = col.GetComponent<UnitController>();
                    unitController.player = gameObject;
                    unitController.TakeThrust(1);
                    unitController.AddBuff(spell.buff, level, 0);
                }

            }
        }

        //if (spell.id == 105) // thrust
        //{

        //    Collider[] _col = Physics.OverlapSphere(transform.TransformPoint(Vector3.forward * 2f), 5f);
        //    Debug.Log("Spell THRUUUUUSTTT");

        //    foreach (var col in _col)
        //    {
        //        if (col.gameObject.tag == "Enemy")
        //        {
        //            UnitController unitController = col.GetComponent<UnitController>();
        //            unitController.player = gameObject;
        //            unitController.TakeThrust(1);
        //            //unitController.AddBuff(spell.buff, level, 0);
        //        }



        //    }
        //}

    }
            IEnumerator DelayColliderDamageEffect(float delay, float radius, float forward, int damage, int damageBuff, Buff buff, GameObject effect, int level)
    {
        yield return new WaitForSeconds(delay);
        if (playerstat.states[0] <= 0)
        {
            Collider[] _col = Physics.OverlapSphere(transform.TransformPoint(Vector3.forward * forward), radius);
            if (effect) Instantiate(effect, transform.TransformPoint(Vector3.forward * forward), transform.rotation);
            foreach (var col in _col)
            {
                if (col.gameObject.tag == "Enemy")
                {
                    UnitController unitController = col.GetComponent<UnitController>();
                    unitController.MakeDamage(damage, 0, 0, 0);
                    if (buff) unitController.AddBuff(buff, level, damageBuff);
                }

            }
        }
    }

    IEnumerator DelayBulletMaker(float delay, Spell spell, int level, int damage)
    {
        yield return new WaitForSeconds(delay);
        if (playerstat.states[0] <= 0)
        {
            for (int i = 0; i < spell.bulletCount; i++)
            {

                GameObject newBullet;

                newBullet = Instantiate(spell.bullet);

                newBullet.transform.rotation = transform.rotation;

                newBullet.transform.position = transform.TransformPoint(Vector3.forward * 0.8f);

                Bullet bulletController = newBullet.GetComponent<Bullet>();
                Animator bulletAnim = newBullet.GetComponent<Animator>();
                bulletController.ally = true;
                if (bulletController.speed <= 7f) bulletController.speed = 7f;
                bulletController.damage = damage;
                bulletController.damageType = spell.typeDamage;
                bulletController.dest = transform.forward;
                bulletController.wait = true;
                bulletController.Wait(0.05f * i);
                if (spell.bulletRandomTraectory == true)
                {
                    int rand = Random.Range(1, 11);

                    bulletAnim.SetInteger("animation", rand);
                }

            }
        }

        
    }


    


}