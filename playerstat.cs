using UnityEngine;
using System.Collections;
using stats;
using UnityEngine.UI;
using UnityEngine.AI;



public class playerstat : MonoBehaviour
{
    // ini
    public Stats stats = new Stats();

    // current hp mp
    public float curHP;
    public float curMP;
    public float curHPpc;
    public float curMPpc;

    // hp/mp bars
    public GameObject HealthBar;
    Image img;
    public Image img2;

    Animator anim;
    SpellMaster spellMaster;
    Inventory inventory;
    PassiveMaster passiveMaster;
    NavMeshAgent nav;

    // buffs
    public BuffsMaster buffsMaster;
    public float[] buffsDebuffs = new float[200];
    public Buff[] buffsList = new Buff[200];
    public GameObject[] effectsOnMe = new GameObject[200];
    public int[] buffsDebuffsStack = new int[200];

    public int[] buffsDebuffsDamage = new int[200];

    public float[] states = new float[25];

   

    // dmg in radius by types
    private int[] dmgRS = new int[10];
    private int[] dmgRM = new int[10];
    private int[] dmgRB = new int[10];

    // dmg me by types
    private int[] dmgMe = new int[10];

    Renderer[] rend;

    public HeroClass heroClass;

    float rate;
    float rateDefense;
    float rateAS;

    public int[] teststats;

    // game menu text stats
    public Text[] menuTextBaseCharack6 = new Text[6];
    public Text[] menuTextMainStats6 = new Text[6];

    public Text[] menuTextDamage7 = new Text[7];
    public Text[] menuTextArmor6 = new Text[6];
    public Text[] menuTextSpell9 = new Text[9];
    public Text[] menuTextTypes8 = new Text[8];
    public Text[] menuTextRes13 = new Text[13];

    //display black
    public GameObject displayBlack;
    Animator animDisplayBlack;

    private void Awake()
    {
        if (Manager.prefabHero)
        {
            var hero = Instantiate(Manager.prefabHero, transform.localPosition, transform.rotation);
            hero.transform.parent = gameObject.transform;
        }
        heroClass = Manager.heroClass;
    }

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        inventory = GetComponent<Inventory>();
        spellMaster = GetComponent<SpellMaster>();
        passiveMaster = GetComponent<PassiveMaster>();
        rend = GetComponentsInChildren<Renderer>();
        nav = GetComponent<NavMeshAgent>();

        animDisplayBlack = displayBlack.GetComponent<Animator>();

        rate = 66 / (((Manager.DoorCount - 1) * 9f) + 100f);
        rateDefense = 80 / (((Manager.DoorCount - 1) * 9f) + 100f);
        rateAS = 2 / (((Manager.DoorCount - 1) * 9f) + 100f);

        // HP BAR     
        img = HealthBar.GetComponent<Image>();

        states = new float[25];


        InvokeRepeating("EverySec", 0, 1);
        InvokeRepeating("FourPerSec", 0, 0.25f);
        
        if (Manager.DoorCount == 1)
        {
            stats.stats[500] = 1;//lvl

            for (int i = 0; i < 100; i++)
            {
                if (heroClass.basicFeature[i] >= 1)
                {
                    stats.stats[100 + i] = heroClass.basicFeature[i];
                    Debug.Log("changing " + heroClass.basicFeature[i]);

                }
            }
        }
        // copy stats / Load hero
        if (Manager.DoorCount > 1)

        {
            stats.stats = Manager.stats;
            spellMaster.spells = Manager.spells;
            inventory.items = Manager.items;
            passiveMaster.passiveListSLot = Manager.passiveListSLot;
            spellMaster.OnSpellsChanged.Invoke();

        }

        stats.newStats();
        //stats.testStats();

        curHP = stats.stats[400];
        Debug.Log(curHP);
        curMP = stats.stats[402];


        img.fillAmount = curHP / stats.stats[400];
        img2.fillAmount = curMP / stats.stats[402];

        anim.SetBool("Run", false);
        anim.SetBool("Idle", true);
        
    }

    void Update()
    {
        if (curHP > stats.stats[400])
            curHP = stats.stats[400];
        if (curHP <= 0)
        {
            // death
            curHP = 0;
            //Time.timeScale = 0.5f;
        }
        if (curMP < 0)
            curMP = 0;
        if (curMP > stats.stats[402])
            curMP = stats.stats[402];

        stats.newStats(); // !!!!!!!!!!!!!!


        curHPpc = curHP / stats.stats[400];
        curMPpc = curMP / stats.stats[402];
        img2.fillAmount = curMPpc;
        img.fillAmount = curHPpc;

        // states
        for (int i = 0; i < 25; i++)
        {
            if (states[i] > 0)
            { states[i] -= Time.deltaTime; if (states[i] <= 0) StateOver(i); }

            if (states[i] <= 0) states[i] = 0;

        }
        ///////////////
        if (Input.GetKeyDown("space"))
        {
            EndOfLevel();
        }
        teststats = stats.stats;
    }

    void StateOver(int id)
    {
        if (id == 1) anim.SetBool("Stun", false);
        if (id == 2)
        {
            nav.enabled = true;
            anim.speed = 1;
            for (int i = 0; i < rend.Length; i++) { rend[i].material = heroClass.matok; }

        }
    }

    public void MakeDamage(int damage, int type, int crit, int thrust)
    {


        if (states[10] <= 0) // make invulnerable state
        {
            if (type == 0)
            {
                //stats.stats[500] += 1;
                //Debug.Log("statsa huinya = " + stats.stats[500]);
                float def = stats.stats[435] * rateDefense;
                if (def > 0.8f) def = 0.8f;

                bool decreaseDamage = false;

                // block
                int random = Random.Range(0, 100);
                if (rate * stats.stats[438] >= random) { curHP = Mathf.Floor(curHP - (((1 - def) * damage) * 0.2f)); passiveMaster.BlockEvent(); decreaseDamage = true; }
                // evade
                random = Random.Range(0, 100);
                if (rate * stats.stats[439] >= random && decreaseDamage == false) { passiveMaster.EvadeEvent(); decreaseDamage = true; }
                // parry
                random = Random.Range(0, 100);
                if (rate * stats.stats[437] >= random && decreaseDamage == false) { curHP = Mathf.Floor(curHP - (((1 - def) * damage) * 0.5f)); passiveMaster.ParryEvent(); decreaseDamage = true; }
                // full damage
                if (decreaseDamage == false) { curHP = Mathf.Floor(curHP - ((1 - def) * damage)); }
            }
            // fire
            if (type == 2)
            {
                float res;
                res = stats.stats[464] / 100;
                if (res > 1)
                {
                    float heal = Mathf.Floor((res - 1) * damage);
                    DealHeal(heal);
                }
                else
                {
                    curHP = Mathf.Floor(curHP - ((1 - res) * damage));
                }
            }
            // ice
            if (type == 3)
            {
                float res;
                res = stats.stats[465] / 100;
                if (res > 1)
                {
                    float heal = Mathf.Floor((res - 1) * damage);
                    DealHeal(heal);
                }
                else
                {
                    curHP = Mathf.Floor(curHP - ((1 - res) * damage));
                }
            }
            // electro
            if (type == 4)
            {
                float res;
                res = stats.stats[466] / 100;
                if (res > 1)
                {
                    float heal = Mathf.Floor((res - 1) * damage);
                    DealHeal(heal);
                }
                else
                {
                    curHP = Mathf.Floor(curHP - ((1 - res) * damage));
                }
            }
            // nature
            if (type == 5)
            {
                float res;
                res = stats.stats[467] / 100;
                if (res > 1)
                {
                    float heal = Mathf.Floor((res - 1) * damage);
                    DealHeal(heal);
                }
                else
                {
                    curHP = Mathf.Floor(curHP - ((1 - res) * damage));
                }
            }
            // poison
            if (type == 6)
            {
                float res;
                res = stats.stats[468] / 100;
                if (res > 1)
                {
                    float heal = Mathf.Floor((res - 1) * damage);
                    DealHeal(heal);
                }
                else
                {
                    curHP = Mathf.Floor(curHP - ((1 - res) * damage));
                }
            }
            // dark arts
            if (type == 7)
            {
                float res;
                res = stats.stats[469] / 100;
                if (res > 1)
                {
                    float heal = Mathf.Floor((res - 1) * damage);
                    DealHeal(heal);
                }
                else
                {
                    curHP = Mathf.Floor(curHP - ((1 - res) * damage));
                }
            }
            // light
            if (type == 8)
            {
                float res;
                res = stats.stats[470] / 100;
                if (res > 1)
                {
                    float heal = Mathf.Floor((res - 1) * damage);
                    DealHeal(heal);
                }
                else
                {
                    curHP = Mathf.Floor(curHP - ((1 - res) * damage));
                }
            }

            if (type >= 1) { passiveMaster.RecievedMagicAttack(); }



        }

    }

    //void DeathInTheSky() { SceneManager.LoadScene("TheGreatHole02"); }

    public void ChangeAttribute(int attribute, int attributeValue)
    {
        float x; float y;
        y = curMP / stats.stats[402];
        x = curHP / stats.stats[400];
        stats.stats[attribute] += attributeValue;

        stats.newStats();
        curMP = y * stats.stats[402];
        curHP = x * stats.stats[400];
    }




    public void AddBuff(Buff buff, int level)

    {
        if (buffsDebuffs[buff.id] <= 0)
        {
            if (buff.isBuff == true)
            {

                buffsMaster.AddBuff(buff);
                buffsDebuffs[buff.id] = buff.duration;
                buffsList[buff.id] = buff;
                buffsDebuffsStack[buff.id] = 1;

                if (buff.isChanging == true)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        if (buff.feature[i] >= 1)
                        {
                            if (level <= 1) ChangeAttribute(200 + i, buff.feature[i]);
                            if (level > 1) ChangeAttribute(200 + i, buff.feature[i] + (buff.forLevelFeature[i] * (level - 1)));

                        }
                    }

                }

                if (buff.effect)
                {
                    var bubble = Instantiate(buff.effect, transform.localPosition + Vector3.up * 0.2f, transform.rotation);
                    bubble.transform.parent = gameObject.transform;
                    effectsOnMe[buff.id] = bubble;
                }
                int damage = 0;

                if (buff.isDamaging)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        if (buff.feature[i] > 0)
                        {
                            if (level == 1) damage += (buff.feature[i] * stats.stats[400 + i]) / 100;
                            if (level > 1) damage += (buff.feature[i] + (buff.forLevelFeature[i] * (level - 1)) * stats.stats[400 + i]) / 100;
                        }
                    }

                    buffsDebuffsDamage[buff.id] = damage / 4;

                    if (buff.radius == 0)
                    {
                        if (buff.typeDamage == 0) dmgRS[0] += buffsDebuffsDamage[buff.id];
                        if (buff.typeDamage == 1) dmgRS[1] += buffsDebuffsDamage[buff.id];
                        if (buff.typeDamage == 2) dmgRS[2] += buffsDebuffsDamage[buff.id];
                    }
                    if (buff.radius == 1)
                    {
                        if (buff.typeDamage == 0) dmgRM[0] += damage;
                        if (buff.typeDamage == 1) dmgRM[1] += damage;
                    }
                    if (buff.radius == 2)
                    {
                        if (buff.typeDamage == 0) dmgRB[0] += damage;
                        if (buff.typeDamage == 1) dmgRB[1] += damage;
                    }

                }

                if (buff.isStates == true)
                {
                    for (int i = 0; i < 25; i++)
                    {
                        if (buff.states[i] > 0) { states[i] = buff.states[i]; }
                        

                    }
                }



            }
            if (buff.isBuff == false && states[10] <= 0)
            {

                buffsMaster.AddBuff(buff);
                buffsDebuffs[buff.id] = buff.duration;
                buffsList[buff.id] = buff;

                if (buff.isChanging == true)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        if (buff.feature[i] >= 1)
                        {
                            ChangeAttribute(200 + i, -buff.feature[i]);
                            //buffsList[buff.id] = buff;

                        }
                    }
                }

                if (buff.isStates == true)
                {
                    for (int i = 0; i < 25; i++)
                    {
                        if (buff.states[i] > 0) { states[i] = buff.states[i]; }

                        if (buff.states[1] > 0) { anim.SetBool("Stun", true); states[0] = buff.states[1]; }

                        if (buff.states[2] > 0) { nav.enabled = false; anim.speed = 0; for (int ii = 0; ii < rend.Length; ii++) rend[ii].material = heroClass.matice; states[0] = buff.states[2]; } // freeze

                        if (buff.states[3] > 0) { anim.SetBool("Knocked", true); states[0] = buff.states[3]; }
                    }
                }

                if (buff.effect)
                {
                    var effect = Instantiate(buff.effect, transform.localPosition + Vector3.up * 0.2f, transform.rotation);
                    effect.transform.parent = gameObject.transform;
                    effectsOnMe[buff.id] = effect;
                }

                if (buff.isDamaging)
                {
                    if (buff.typeDamage == 0) dmgMe[0] += buff.damage;
                    if (buff.typeDamage == 1) dmgMe[1] += buff.damage;

                }




            }


        }
        if (buffsDebuffs[buff.id] > 0 && buff.isStacking == true)
        {

            if (buffsDebuffsStack[buff.id] < buff.maxStack)
            {
                buffsDebuffsStack[buff.id] += 1;
                buffsDebuffs[buff.id] = buff.duration;
                for (int i = 0; i < 100; i++)
                {
                    if (buff.feature[i] >= 1)
                    {
                        ChangeAttribute(200 + i, buff.feature[i]);
                        Debug.Log("stacking motherfucker");

                    }
                }
            }
        }
        if (buffsDebuffs[buff.id] > 0 && buff.isStacking == false) { buffsDebuffs[buff.id] = buff.duration; }


    }

    // HERE

    public void MenuClick()
    {
        ///// MAIN /////
        // hp 1
        menuTextMainStats6[0].text = curHP + " / " + stats.stats[400]; 
        // mp 2
        menuTextMainStats6[1].text = curMP + " / " + stats.stats[402];
        // m damage 3 
        if (heroClass.mainAttribute == 0) { menuTextMainStats6[2].text = "" + stats.stats[471]; menuTextDamage7[0].text = "" + stats.stats[471]; }
        if (heroClass.mainAttribute == 1) { menuTextMainStats6[2].text = "" + stats.stats[472]; menuTextDamage7[0].text = "" + stats.stats[472]; }
        if (heroClass.mainAttribute == 2) { menuTextMainStats6[2].text = "" + stats.stats[473]; menuTextDamage7[0].text = "" + stats.stats[473]; }
        if (heroClass.mainAttribute == 3) { menuTextMainStats6[2].text = "" + stats.stats[474]; menuTextDamage7[0].text = "" + stats.stats[474]; }
        if (heroClass.mainAttribute == 4) { menuTextMainStats6[2].text = "" + stats.stats[475]; menuTextDamage7[0].text = "" + stats.stats[475]; }
                    // r damage 4
        if (heroClass.mainAttribute == 0) { menuTextMainStats6[3].text = "" + stats.stats[481]; menuTextDamage7[1].text = "" + stats.stats[481]; }
        if (heroClass.mainAttribute == 1) { menuTextMainStats6[3].text = "" + stats.stats[482]; menuTextDamage7[1].text = "" + stats.stats[482]; }
        if (heroClass.mainAttribute == 2) { menuTextMainStats6[3].text = "" + stats.stats[483]; menuTextDamage7[1].text = "" + stats.stats[483]; }
        if (heroClass.mainAttribute == 3) { menuTextMainStats6[3].text = "" + stats.stats[484]; menuTextDamage7[1].text = "" + stats.stats[484]; }
        if (heroClass.mainAttribute == 4) { menuTextMainStats6[3].text = "" + stats.stats[485]; menuTextDamage7[1].text = "" + stats.stats[485]; }
        // defense 5
        menuTextMainStats6[4].text =  "" + (rateDefense * stats.stats[441]) + "%"; menuTextArmor6[5].text = "" + (rateDefense * stats.stats[441]) + "%"; menuTextRes13[3].text = "" + (rateDefense * stats.stats[441]) + "%";
        // spell 6
        menuTextMainStats6[5].text = "" + stats.stats[442];


        ///// BASE CHARACKTERISTIKS /////
        // str 1
        menuTextBaseCharack6[0].text = "" + stats.stats[404];
        // agi 2
        menuTextBaseCharack6[1].text = "" + stats.stats[406];
        // int 3 
        menuTextBaseCharack6[2].text = "" + stats.stats[412];
        // stamina 4
        menuTextBaseCharack6[3].text = "" + stats.stats[408];
        // spirit 5
        menuTextBaseCharack6[4].text = "" + stats.stats[410];
        // luck 6
        menuTextBaseCharack6[5].text = "" + stats.stats[414];

        ///// DAMAGE /////
        // m damage 1
        // r damage 2
        // as 3 
        menuTextDamage7[2].text = "" + (((stats.stats[430] * rateAS) + 1) * stats.stats[417]);
        // mastery 4
        menuTextDamage7[3].text = "" + stats.stats[431];
        // crit chance 5
        menuTextDamage7[4].text = "" + stats.stats[432] + "%";
        // crit dam 6
        menuTextDamage7[5].text = "" + stats.stats[433] + "%";
        // arp 7
        menuTextDamage7[5].text = "" + stats.stats[434];

        ///// ARMOR /////
        // armor 1
        menuTextArmor6[0].text = "" + stats.stats[435];
        // parry 2
        menuTextArmor6[1].text = "" + (rate * stats.stats[437]) + "%";
        // block 3 
        menuTextArmor6[2].text = "" + (rate * stats.stats[438]) + "%";
        // evade 4
        menuTextArmor6[3].text = "" + (rate * stats.stats[439]) + "%";
        // hp reg 5
        menuTextArmor6[4].text = "" + stats.stats[440];
        // phys def 6
        
        ///// SPELL /////
        // spell power 1
        menuTextSpell9[0].text = "" + stats.stats[442];
        // spell penetr 2
        menuTextSpell9[1].text = "" + stats.stats[443];
        // spell speed 3 
        menuTextSpell9[2].text = "" + stats.stats[444];
        // spell damage 4
        menuTextSpell9[3].text = "" + stats.stats[445];
        // spell healing 5
        menuTextSpell9[4].text = "" + stats.stats[447];
        // crit chance 6
        menuTextSpell9[5].text = "" + stats.stats[449] + "%";
        // sell crit damage 7
        menuTextSpell9[6].text = "" + stats.stats[450] + "%";
        // mp reg 8
        menuTextSpell9[7].text = "" + stats.stats[451];
        // concentration 9
        menuTextSpell9[8].text = "" + stats.stats[453];

        ///// TYPES /////
        // fire 1
        menuTextTypes8[0].text = "" + stats.stats[454] + "%";
        // ice 2
        menuTextTypes8[1].text = "" + stats.stats[455] + "%";
        // electr 3 
        menuTextTypes8[2].text = "" + stats.stats[456] + "%";
        // nature 4
        menuTextTypes8[3].text = "" + stats.stats[457] + "%";
        // poison 5
        menuTextTypes8[4].text = "" + stats.stats[458] + "%";
        // dark 6
        menuTextTypes8[5].text = "" + stats.stats[459] + "%";
        // light 7
        menuTextTypes8[6].text = "" + stats.stats[460] + "%";
        // bleed 8
        menuTextTypes8[7].text = "0%";

        ///// RES /////
        // phys res 1
        menuTextRes13[0].text = "" + stats.stats[461];
        // magic res 2
        menuTextRes13[1].text = "" + stats.stats[462];
        // mental 3 
        menuTextRes13[2].text = "" + stats.stats[463];
        // phys def
        // magic def
        menuTextRes13[4].text = "" + (rateDefense * stats.stats[452]) + "%";
        // fire 1
        menuTextRes13[5].text = "" + stats.stats[464] + "%";
        // ice 2
        menuTextRes13[6].text = "" + stats.stats[465] + "%";
        // electr 3 
        menuTextRes13[7].text = "" + stats.stats[466] + "%";
        // nature 4
        menuTextRes13[8].text = "" + stats.stats[467] + "%";
        // poison 5
        menuTextRes13[9].text = "" + stats.stats[468] + "%";
        // dark 6
        menuTextRes13[10].text = "" + stats.stats[469] + "%";
        // light 7
        menuTextRes13[11].text = "" + stats.stats[470] + "%";
        // bleed 8
        menuTextRes13[12].text = "0%";
    }

    public void DealHeal(float heal) { curHP += heal; }
    public void DealMana(float mana) { curMP += mana; }
    public void ReduceHealth(float health) { curHP -= health; }
    public void ReduceMana(float mana) { curMP -= mana; }
    public void DealState(int state, float durition) { states[state] = durition; }


    private void EverySec()
    {


        curHP += stats.stats[440]; //health reg
        curMP += stats.stats[451]; //mana reg

        for (int i = 0; i < 200; i++)
        {
            if (buffsDebuffs[i] >= 1)
            {
                buffsDebuffs[i] -= 1;

                if (buffsDebuffs[i] <= 0)
                {
                    if (buffsList[i].isBuff == true)
                    {
                        if (buffsList[i].isDamaging)
                        {
                            if (buffsList[i].radius == 0)
                            {
                                if (buffsList[i].typeDamage == 0) dmgRS[0] -= buffsDebuffsDamage[i];
                                if (buffsList[i].typeDamage == 1) dmgRS[1] -= buffsDebuffsDamage[i];
                                if (buffsList[i].typeDamage == 2) dmgRS[2] -= buffsDebuffsDamage[i];
                            }
                            if (buffsList[i].radius == 1)
                            {
                                if (buffsList[i].typeDamage == 0) dmgRM[0] -= buffsList[i].damage;
                                if (buffsList[i].typeDamage == 1) dmgRM[1] -= buffsList[i].damage;
                            }
                            if (buffsList[i].radius == 2)
                            {
                                if (buffsList[i].typeDamage == 0) dmgRB[0] -= buffsList[i].damage;
                                if (buffsList[i].typeDamage == 1) dmgRB[1] -= buffsList[i].damage;
                            }
                        }
                        if (buffsList[i].isChanging && buffsList[i].isStacking == false)
                        {
                            for (int ii = 0; ii < 100; ii++)
                            {
                                if (buffsList[i].feature[ii] >= 1) { ChangeAttribute(200 + ii, -buffsList[i].feature[ii]); }
                                if (ii == 99) { RemoveBuff(i); }
                            }
                        }
                        if (buffsList[i].isChanging && buffsList[i].isStacking == true)
                        {
                            for (int ii = 0; ii < 100; ii++)
                            {
                                if (buffsList[i].feature[ii] >= 1) { ChangeAttribute(200 + ii, -buffsList[i].feature[ii] * (buffsDebuffsStack[i])); }
                                if (ii == 99) { RemoveBuff(i); }
                            }
                            buffsDebuffsStack[i] = 0;
                        }

                        else RemoveBuff(i);
                    }
                    else
                    {
                        if (buffsList[i].isDamaging)
                        {
                            if (buffsList[i].typeDamage == 0) dmgMe[0] -= buffsList[i].damage;
                            if (buffsList[i].typeDamage == 1) dmgMe[1] -= buffsList[i].damage;
                            if (buffsList[i].typeDamage == 2) dmgMe[2] -= buffsList[i].damage;
                        }
                        if (buffsList[i].isChanging)
                        {
                            for (int ii = 0; ii < 100; ii++)
                            {
                                if (buffsList[i].feature[ii] >= 1) { ChangeAttribute(200 + ii, +buffsList[i].feature[ii]); }
                                if (ii == 99) { RemoveBuff(i); }
                            }
                        }
                        else { RemoveBuff(i); }
                    }


                }
            }
        } // -1 duration for buffs, and if some buff is done - clear



    }
    

    private void FourPerSec()
    {

        float y = stats.stats[417] / 100f;
        float x = stats.stats[430] * rateAS;
        //Debug.Log(" yyyyy =  =   " + y);
        float a = (1 + x) * y;
        if (a > 3) a = 3;
        //Debug.Log(" aaaa =  =   " + a);
        anim.SetFloat("SpeedAttack", a);

        if (dmgRS[0] > 0 || dmgRS[1] > 0 || dmgRS[2] > 0 || states[20] > 0)
        {
            Collider[] _col = Physics.OverlapSphere(transform.position, 2.4f);
            //Debug.Log("54444444444444444444    " );
            foreach (var col in _col)
            {
                if (col.gameObject.tag == "Enemy")
                {
                    UnitController unitController = col.GetComponent<UnitController>();
                    if (dmgRS[0] > 0) unitController.MakeDamage(dmgRS[0], 0, 0, 0);
                    if (dmgRS[1] > 0) unitController.MakeDamage(dmgRS[1], 1, 0, 0);
                    if (dmgRS[2] > 0) unitController.MakeDamage(dmgRS[2], 2, 0, 0);


                    if (states[20] > 0) { unitController.player = gameObject; unitController.TakeThrust(1f); unitController.AddBuff(buffsList[17].buffTarget, 1, 0); }
                }
                

            }
        }
        if (dmgMe[0] > 0 || dmgMe[1] > 0 || dmgMe[2] > 0)
        {
            if (dmgMe[0] > 0) MakeDamage(dmgMe[0], 0, 0, 0);
            if (dmgMe[1] > 0) MakeDamage(dmgMe[1], 1, 0, 0);
            if (dmgMe[2] > 0) MakeDamage(dmgMe[2], 2, 0, 0);
        }



        
    }

    public void RemoveBuff(int i)
    {
        buffsMaster.RemoveBuff(buffsList[i]);
        buffsList[i] = null;
        buffsDebuffs[i] = 0;
        Destroy(effectsOnMe[i]);
        effectsOnMe[i] = null;

    }

    public void EndOfLevel()
    {
        for (int i = 0; i < 100; i++)
        {
            stats.stats[i + 200] = 0;
            //if (buffsList[i] != null) RemoveBuff(i);
            //if (buffsList[i+100] != null) RemoveBuff(i+100);
            buffsList[i] = null;
            buffsList[i + 100] = null;
            buffsDebuffs[i] = 0;
            buffsDebuffs[i + 100] = 0;
        }

        for (int i = 0; i < 100; i++)
        {
            if (heroClass.forLevelFeature[i] > 0) stats.stats[i + 100] += heroClass.forLevelFeature[i];

        }
    
        stats.stats[499] += 3;
        Manager.stats = stats.stats;
        Manager.spells = spellMaster.spells;
        Manager.items = inventory.items;
        Manager.passiveListSLot = passiveMaster.passiveListSLot;

        animDisplayBlack.SetTrigger("EndOfLevel");

        StartCoroutine(EndEnd());

    }

    IEnumerator EndEnd()
    {

        yield return new WaitForSeconds(1.5f);
        Manager.EndLevel();

    }

}