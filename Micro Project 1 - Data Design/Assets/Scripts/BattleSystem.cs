using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST}
public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;

    public Transform PlayerBattleStation;
    public Transform EnemyBattleStation;

    Unit PlayerUnit;
    Unit EnemyUnit;

    public TMP_Text DialogueText;
    public TMP_Text CardCounterText;

    public BattleHUD PlayerHUD;
    public BattleHUD EnemyHUD;

    public GameObject buttons;

    public GameObject MainUIStuff;
    public GameObject EndScene;

    public Animator EnemyAnim;
    public Animator PlayerAnim;
   // public Animator CamAnim;
    public Animator pageAnim;
    //public Animator RiseAnim;

    public GameObject HealEffect;
    public GameObject LowerEffect;
    public GameObject RiseEffect;

    public GameObject EnemyEffectSpawnPoint;
    public GameObject PlayerEffectSpawnPoint;
    public GameObject EnemyEffectSpawnPointLower;
    public GameObject PlayerEffectSpawnPointLower;

    public int PlayNextCounter = 0;

    public int HealAmount;
    public int SmokeAmount;
    public int DoppelAmount;

    public GameObject btnDesc;

    public GameObject BattleStart;

    public GameObject GateCardScene;
    public GameObject GateCard1;
    public GameObject GateCard2;
    public GameObject GateCard3;

    //public int PlayerGateCardsWon;
    //public int EnemyGateCardsWon;

    public GameObject ActivateSkipCard;
    public GameObject ActivateSwitchCard;
    public GameObject ActivateStealCard;
    public GameObject GateCardActivated;
    public GameObject EnemyGateCardActivated;

    public GameObject PlayerCounterCard1;
    public GameObject EnemyCounterCard1;
    public GameObject PlayerCounterCard2;
    public GameObject EnemyCounterCard2;
    public GameObject PlayerCounterCard3;
    public GameObject EnemyCounterCard3;

    public static int CardCounter;
    public static int EnemyCardCounter;

    public GameObject Error;

    Scene currentScene;

    private int EnemyTemp;
    private int PlayerTemp;

    public bool isGateCardActivated;

    public int EnemyCardCounterActivated = 1;

    //private IEnumerator Enemy; 
    //private IEnumerator Player;

    private void Start()
    {
        //Enemy = EnemyTurn();
        //Player = PlayerTurn();

        currentScene = SceneManager.GetActiveScene();

        state = BattleState.START;
        buttons.SetActive(false);
        btnDesc.SetActive(false);
        BattleStart.SetActive(true);
       
        SetupBattle();
        StartCoroutine(SetupBattle());

        EnemyAnim.SetBool("EnemyHit", false);
        EnemyAnim.SetBool("EnemySmoked", false);
        EnemyAnim.SetBool("EnemySplits", false);
        EnemyAnim.SetBool("EnemyHeals", false);

        PlayerAnim.SetBool("PlayerHit", false);
        PlayerAnim.SetBool("PlayerSmoked", false);
        PlayerAnim.SetBool("PlayerSplits", false);
        PlayerAnim.SetBool("PlayerHeals", false);

        //RiseAnim.SetBool("isDoneIncreasing", true);

        ActivateSkipCard.SetActive(false);
        ActivateSwitchCard.SetActive(false);
        ActivateStealCard.SetActive(false);

    }

    IEnumerator SetupBattle()
    {
        yield return new WaitForSeconds(1f);
        BattleStart.SetActive(false);

        GameObject PlayerGo = Instantiate(PlayerPrefab, PlayerBattleStation);
        PlayerUnit = PlayerGo.GetComponent<Unit>();

        GameObject EnemeyGo = Instantiate(EnemyPrefab, EnemyBattleStation);
        EnemyUnit = EnemeyGo.GetComponent<Unit>();

        DialogueText.text = EnemyUnit.UnitName + " Has Appeared!";
        CardCounterText.text = "Gate Cards Won: " + CardCounter;



        PlayerHUD.SetHUD(PlayerUnit);
        EnemyHUD.SetHUD(EnemyUnit);
        

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;

        StartCoroutine(PlayerTurn());
    }

    IEnumerator PlayerAttack()
    {
        buttons.SetActive(false);
        btnDesc.SetActive(false);

        EnemyUnit.CurrentAcc = Random.Range(EnemyUnit.CurrentAcc, EnemyUnit.MaxAcc);

        //Debug.Log("Enemy Acc: " + EnemyUnit.CurrentAcc);
        if (EnemyUnit.CurrentSpeed >= PlayerUnit.CurrentAcc)
        {
            yield return new WaitForSeconds(2f);
            bool isDead = EnemyUnit.TakeDamage(0);

            
            EnemyHUD.SetHP(EnemyUnit.CurrentHP);
            PlayerHUD.StatsText.text = "HP: " + PlayerUnit.CurrentHP;
            EnemyHUD.StatsText.text = "HP: " + EnemyUnit.CurrentHP;

            DialogueText.text = "Oh No! Enemy Has Dodged!";
            

            yield return new WaitForSeconds(3f);

            if (isDead) //the enemy
            {
                state = BattleState.WON;
                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }
        else if(EnemyUnit.CurrentSpeed <= PlayerUnit.CurrentAcc)
        {
            EnemyAnim.SetBool("EnemyHit", true);

            bool isDead = EnemyUnit.TakeDamage(PlayerUnit.Damage);

            EnemyHUD.SetHP(EnemyUnit.CurrentHP);

            DialogueText.text = "Enemy Has Been Hit!";
          

            yield return new WaitForSeconds(3f);

            if (isDead) //the enemy
            {
                state = BattleState.WON;
                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }
        }

    }

    IEnumerator PlayerHeal()
    {
        buttons.SetActive(false);
        btnDesc.SetActive(false);


        if (PlayerUnit.CurrentHP < PlayerUnit.MaxHP)
        {
            DialogueText.text = "You Have Recovered Energy!";
            PlayerAnim.SetBool("PlayerHeals", false);
            //RiseAnim.SetBool("isDoneIncreasing", true);
            Instantiate(HealEffect,PlayerEffectSpawnPoint.transform.position, Quaternion.identity);
        }
        else
        {
            DialogueText.text = "You Are Already Fully Healed.";
        }

        yield return new WaitForSeconds(4f);

        PlayerUnit.Heal(HealAmount);
        PlayerHUD.SetHP(PlayerUnit.CurrentHP);
        PlayerHUD.StatsText.text = "HP: " + PlayerUnit.CurrentHP;
        EnemyHUD.StatsText.text = "HP: " + EnemyUnit.CurrentHP;

        yield return new WaitForSeconds(3f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
    
     IEnumerator PlayerSmoke() 
    {
        EnemyAnim.SetBool("EnemySmoked", true);

        buttons.SetActive(false);
        btnDesc.SetActive(false);

        EnemyUnit.SmokeSpeed(SmokeAmount);
        PlayerUnit.SmokeDef(SmokeAmount);

        //EnemyHUD.StatsText.text = "Accuracy: " + EnemyUnit.CurrentAcc + "%" + "\n" + "Speed: " + EnemyUnit.CurrentSpeed;
        //PlayerHUD.StatsText.text = "Accuracy: " + PlayerUnit.CurrentAcc + "%" + "\n" + "Speed: " + PlayerUnit.CurrentSpeed;

        EnemyUnit.CurrentSpeed = Random.Range(EnemyUnit.CurrentSpeed, EnemyUnit.MaxSpeed);

        if (EnemyUnit.CurrentSpeed >= EnemyUnit.MaxSpeed)
        {
            DialogueText.text = "Enemy's SPEED cannot rise anymore!"; //\n Enemy DEF. increased!";

        }
        else if(EnemyUnit.CurrentSpeed <= 0)
        {
            DialogueText.text = "Enemy SPEED cannot lower anymore! \n \n DEF. has increased!"; //\n Enemy DEF. increased!";
            Instantiate(RiseEffect, EnemyEffectSpawnPoint.transform.position, Quaternion.identity);
            
        }
        else if(EnemyUnit.CurrentDef >= EnemyUnit.MaxDef)
        {
            DialogueText.text = "Enemy's SPEED has lowered \n \n Enemy DEF. cannot rise anymore!"; //\n Enemy SPEED lowered!";
            Instantiate(LowerEffect, EnemyEffectSpawnPointLower.transform.position, Quaternion.identity);
        }
        else if (EnemyUnit.CurrentDef <= 0)
        {
            DialogueText.text = "Enemy DEF. cannot lower anymore!";// \n Enemy SPEED risen!";
            
        }
        else
        {
            DialogueText.text = "Enemy's SPEED has lowered \n \n DEF. has increased!";
            yield return new WaitForSeconds(4f);

            Instantiate(LowerEffect, EnemyEffectSpawnPointLower.transform.position, Quaternion.identity);
            
            Instantiate(RiseEffect, EnemyEffectSpawnPoint.transform.position, Quaternion.identity);

        }

        yield return new WaitForSeconds(4f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
      }

    IEnumerator PlayerDoppel()
    {
        PlayerAnim.SetBool("PlayerSplits", true);

        buttons.SetActive(false);
        btnDesc.SetActive(false);

        EnemyUnit.Doppel(DoppelAmount);

        //EnemyHUD.StatsText.text = "Accuracy: " + EnemyUnit.CurrentAcc + "%" + "\n" + "Speed: " + EnemyUnit.CurrentSpeed;

        EnemyUnit.CurrentDef = Random.Range(EnemyUnit.CurrentDef, EnemyUnit.MaxDef);

        if (EnemyUnit.Damage >= EnemyUnit.MaxDamage)
        {
            DialogueText.text = "Enemy's ACCURACY has lowered. \n \n Enemy ATTACK DAMAGE cannot rise anymore.";// + "\n" + "Enemy ACCURACY lowered!";
            Instantiate(LowerEffect, EnemyEffectSpawnPointLower.transform.position, Quaternion.identity);

        }
        else if (EnemyUnit.Damage <= 0)
        {
            DialogueText.text = "Enemy ATTACK DAMAGE cannot lower anymore.";// + "\n" + "Enemy ACCURACY lowered!";
        }
        else if (EnemyUnit.CurrentAcc >= EnemyUnit.MaxAcc)
        {
            DialogueText.text = "Enemy ACCURACY cannot lower anymore.\n \n ATTACK DAMAGE has increased!";// + "\n" + "Enemy ATTACK DAMAGE increased!";
            Instantiate(RiseEffect, EnemyEffectSpawnPoint.transform.position, Quaternion.identity);

        }
        else if (EnemyUnit.CurrentAcc <= 0)
        {
            DialogueText.text = "Enemy ACCURACY cannot lower anymore.";// + "\n" + "Enemy ATTACK DAMAGE increased!";
        }
        else
        {
            DialogueText.text = "Enemy's ACCURACY has lowered \n \n ATTACK DAMAGE has increased!";
            yield return new WaitForSeconds(4f);

            Instantiate(LowerEffect, EnemyEffectSpawnPointLower.transform.position, Quaternion.identity);

            Instantiate(RiseEffect, EnemyEffectSpawnPoint.transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(4f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            CardCounter++;

            MainUIStuff.SetActive(false);

            if (currentScene.name == "Battle3NotEvolved" || currentScene.name == "Battle3")
            {
                SceneManager.LoadScene(6);
            }
            else
            {
                EndScene.SetActive(true);
            }

            DialogueText.text = EnemyUnit.UnitName + " Has Been Defeated! You Win the Gate Card";
            buttons.SetActive(false);
            btnDesc.SetActive(false);

            if (currentScene.name == "MicroProject3")
            {
                if (CardCounter == 1)
                {
                    PlayerCounterCard1.SetActive(true);
                }
                else if (CardCounter == 2)
                {
                    PlayerCounterCard2.SetActive(true);
                }
                else if (CardCounter == 3)
                {
                    PlayerCounterCard3.SetActive(true);
                }
                else if(CardCounter == 0)
                {
                    PlayerCounterCard1.SetActive(true);
                }

            }
            else if (currentScene.name == "Battle2")
            {
                if (CardCounter == 1)
                {
                    PlayerCounterCard1.SetActive(true);
                }
                else if (CardCounter == 2)
                {
                    PlayerCounterCard2.SetActive(true);
                }
                else if (CardCounter == 3)
                {
                    PlayerCounterCard3.SetActive(true);
                }
                else if (CardCounter == 0)
                {
                    PlayerCounterCard1.SetActive(true);
                }
            }
            else if (currentScene.name == "Battle3")
            {
                if (CardCounter == 1)
                {
                    PlayerCounterCard1.SetActive(true);
                }
                else if (CardCounter == 2)
                {
                    PlayerCounterCard2.SetActive(true);
                }
                else if (CardCounter == 3)
                {
                    PlayerCounterCard3.SetActive(true);
                }
                else if (CardCounter == 0)
                {
                    PlayerCounterCard1.SetActive(true);
                }
            }
            else if (currentScene.name == "Battle3NotEvolved")
            {
                if (CardCounter == 1)
                {
                    PlayerCounterCard1.SetActive(true);
                }
                else if (CardCounter == 2)
                {
                    PlayerCounterCard2.SetActive(true);
                }
                else if (CardCounter == 3)
                {
                    PlayerCounterCard3.SetActive(true);
                }
                else if (CardCounter == 0)
                {
                    PlayerCounterCard1.SetActive(true);
                }
            }
            
        }
        else if(state == BattleState.LOST)
        {
            EnemyCardCounter++;
            Debug.Log("Enemy card count: " + EnemyCardCounter);

            MainUIStuff.SetActive(false);

            if (currentScene.name == "Battle3NotEvolved" || currentScene.name == "Battle3")
            {
                SceneManager.LoadScene(5);
            }
            else
            {
                EndScene.SetActive(true);
            }

            DialogueText.text = "You Lost...";
            buttons.SetActive(false);
            btnDesc.SetActive(false);

            if (currentScene.name == "MicroProject3")
            {
                if (EnemyCardCounter == 1)
                {
                    EnemyCounterCard1.SetActive(true);
                }
                else if (EnemyCardCounter == 2)
                {
                    EnemyCounterCard2.SetActive(true);
                }
                else if (EnemyCardCounter == 3)
                {
                    SceneManager.LoadScene(5);
                }
                else if (EnemyCardCounter == 0)
                {
                    EnemyCounterCard1.SetActive(true);
                }

            }
            else if (currentScene.name == "Battle2")
            {
                if (EnemyCardCounter == 1)
                {
                    EnemyCounterCard1.SetActive(true);
                }
                else if (EnemyCardCounter == 2)
                {
                    EnemyCounterCard2.SetActive(true);
                }
                else if (EnemyCardCounter == 3)
                {
                    SceneManager.LoadScene(5);
                }
                else if (EnemyCardCounter == 0)
                {
                    EnemyCounterCard1.SetActive(true);
                }
            }
            else if (currentScene.name == "Battle3")
            {
                if (EnemyCardCounter == 1)
                {
                    EnemyCounterCard1.SetActive(true);
                }
                else if (EnemyCardCounter == 2)
                {
                    EnemyCounterCard2.SetActive(true);
                }
                else if (EnemyCardCounter == 3)
                {
                    SceneManager.LoadScene(5);
                }
                else if (EnemyCardCounter == 0)
                {
                    EnemyCounterCard1.SetActive(true);
                }
            }
            else if (currentScene.name == "Battle3NotEvolved")
            {
                if (EnemyCardCounter == 1)
                {
                    EnemyCounterCard1.SetActive(true);
                }
                else if (EnemyCardCounter == 2)
                {
                    EnemyCounterCard2.SetActive(true);
                }
                else if (EnemyCardCounter == 3)
                {
                    SceneManager.LoadScene(5);
                }
                else if (EnemyCardCounter == 0)
                {
                    EnemyCounterCard1.SetActive(true);
                }
            }

        }
        
    }

    IEnumerator PlayerTurn()
    {
        PlayerHUD.StatsText.text = "HP: " + PlayerUnit.CurrentHP;
        EnemyHUD.StatsText.text = "HP: " + EnemyUnit.CurrentHP;

        yield return new WaitForSeconds(1f);

        PlayerAnim.SetBool("PlayerHit", false);
        PlayerAnim.SetBool("PlayerHeals", false);
        PlayerAnim.SetBool("PlayerSplits", false);
        PlayerAnim.SetBool("PlayerSmoked", false);

        EnemyAnim.SetBool("EnemyHit", false);
        EnemyAnim.SetBool("EnemySmoked", false);
        EnemyAnim.SetBool("EnemyHeals", false);
        EnemyAnim.SetBool("EnemySplits", false);

        buttons.SetActive(true);

        btnDesc.SetActive(true);

        GateCardActivated.SetActive(false);

        DialogueText.text = "What Move Will You Choose?";
        PlayerHUD.StatsText.text = "HP: " + PlayerUnit.CurrentHP;
        EnemyHUD.StatsText.text = "HP: " + EnemyUnit.CurrentHP;

        /*if (currentScene.name == "Battle3" || currentScene.name == "Battle3NotEvolved")
        {
            Destroy(PlayerCounterCard1);
            Destroy(EnemyCounterCard1);
            Destroy(PlayerCounterCard2);
            Destroy(EnemyCounterCard2);
            Destroy(PlayerCounterCard3);
            Destroy(EnemyCounterCard3);
        }*/

        
        /*if (isGateCardActivated == true)
        {
            GateCardActivated.SetActive(true);

            yield return new WaitForSeconds(2f);
            GateCardActivated.SetActive(false);
            ActivateSkipCard.SetActive(false);

            StartCoroutine(Skip());
            
        }*/

        yield return new WaitForSeconds(0f);
    }

    



    IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy turn starts: " + BattleState.ENEMYTURN);
        yield return new WaitForSeconds(0f);

        PlayerHUD.StatsText.text = "HP: " + PlayerUnit.CurrentHP;
        EnemyHUD.StatsText.text = "HP: " + EnemyUnit.CurrentHP;


        if (currentScene.name == "Battle3" && EnemyUnit.CurrentHP <= 135 && state == BattleState.ENEMYTURN && EnemyCardCounterActivated == 1 || currentScene.name == "Battle3NotEvolved" && EnemyUnit.CurrentHP <= 135 && state == BattleState.ENEMYTURN && EnemyCardCounterActivated == 1)
        {
            EnemyCardCounterActivated--;
            OnBossTurnGateCard();

        }
        else if (currentScene.name == "Battle2" && state == BattleState.ENEMYTURN && EnemyUnit.CurrentHP <= 50 && EnemyCardCounterActivated == 1)
        {
            EnemyCardCounterActivated--;
            OnEnemyTurnGateCard();
        }

        EnemyAnim.SetBool("EnemyHit", false);
        EnemyAnim.SetBool("EnemySmoked", false);
        EnemyAnim.SetBool("EnemyHeals", false);
        EnemyAnim.SetBool("EnemySplits", false);

        PlayerAnim.SetBool("PlayerHit", false);
        PlayerAnim.SetBool("PlayerHeals", false);
        PlayerAnim.SetBool("PlayerSplits", false);
        PlayerAnim.SetBool("PlayerSmoked", false);

        buttons.SetActive(false);

        btnDesc.SetActive(false);

        PlayerHUD.StatsText.text = "HP: " + PlayerUnit.CurrentHP;
        EnemyHUD.StatsText.text = "HP: " + EnemyUnit.CurrentHP;

        PlayerUnit.CurrentSpeed = Random.Range(PlayerUnit.CurrentSpeed, PlayerUnit.MaxSpeed);
        PlayerUnit.CurrentDef = Random.Range(PlayerUnit.CurrentDef, PlayerUnit.MaxDef);
        PlayerUnit.CurrentAcc = Random.Range(PlayerUnit.CurrentAcc, PlayerUnit.MaxAcc);

        //Debug.Log("Player Speed: " + PlayerUnit.CurrentSpeed);
        if (PlayerUnit.CurrentSpeed >= EnemyUnit.CurrentAcc)
        {
            DialogueText.text = EnemyUnit.UnitName + " ATTACKS!";
            yield return new WaitForSeconds(3f);

            bool isDead = PlayerUnit.TakeDamage(0);

            PlayerHUD.SetHP(PlayerUnit.CurrentHP);
            PlayerHUD.StatsText.text = "HP: " + PlayerUnit.CurrentHP;
            EnemyHUD.StatsText.text = "HP: " + EnemyUnit.CurrentHP;

            DialogueText.text = "You Dodged!";

            yield return new WaitForSeconds(3f);

            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                StartCoroutine(PlayerTurn());
            }
        }
        else if(EnemyUnit.CurrentHP <= 2/EnemyUnit.MaxHP) //if enemy's hp below half then heal
        {
            EnemyAnim.SetBool("EnemyHeals", true);

            DialogueText.text = EnemyUnit.UnitName + " HEALS itself!";
            yield return new WaitForSeconds(1f);

            if (EnemyUnit.CurrentHP < EnemyUnit.MaxHP)
            {
                DialogueText.text = "Enemy Recovered Energy!";
                Instantiate(HealEffect, EnemyEffectSpawnPoint.transform.position, Quaternion.identity);
            }
            else
            {
                DialogueText.text = "Enemy is Fully Healed.";
            }
            yield return new WaitForSeconds(4f);

            EnemyUnit.Heal(HealAmount);
            EnemyHUD.SetHP(EnemyUnit.CurrentHP);

            yield return new WaitForSeconds(3f);

            state = BattleState.PLAYERTURN;
            StartCoroutine(PlayerTurn());
        }
        else if (PlayerUnit.CurrentSpeed > EnemyUnit.CurrentSpeed && PlayNextCounter == 1)
        {
            PlayerAnim.SetBool("PlayerSmoked", true);

            DialogueText.text = EnemyUnit.UnitName + " Releases a dark, ominous SMOKE!";
            yield return new WaitForSeconds(2f);

            PlayerUnit.SmokeSpeed(SmokeAmount);
            EnemyUnit.SmokeDef(SmokeAmount);

            //PlayerHUD.StatsText.text = "Accuracy: " + PlayerUnit.CurrentAcc + "%" + "\n" + "Speed: " + PlayerUnit.CurrentSpeed;
            //EnemyHUD.StatsText.text = "Accuracy: " + EnemyUnit.CurrentAcc + "%" + "\n" + "Speed: " + EnemyUnit.CurrentSpeed;

            if (PlayerUnit.CurrentSpeed >= PlayerUnit.MaxSpeed)
            {
                DialogueText.text = "Your SPEED cannot rise anymore!"; 

            }
            else if (PlayerUnit.CurrentSpeed <= 0)
            {
                DialogueText.text = "Your SPEED cannot lower anymore! \n \n DEF. has increased!";
                Instantiate(RiseEffect, PlayerEffectSpawnPoint.transform.position, Quaternion.identity);

            }
            else if (PlayerUnit.CurrentDef >= PlayerUnit.MaxDef)
            {
                DialogueText.text = "Your SPEED has lowered \n \n Your DEF. cannot rise anymore!";
                Instantiate(LowerEffect, PlayerEffectSpawnPointLower.transform.position, Quaternion.identity);

            }
            else if (PlayerUnit.CurrentDef <= 0)
            {
                DialogueText.text = "Your DEF. cannot lower anymore!";
            }
            else
            {
                DialogueText.text = "Your SPEED has lowered \n \n DEF. has increased!";
                yield return new WaitForSeconds(2f);

                Instantiate(LowerEffect, PlayerEffectSpawnPointLower.transform.position, Quaternion.identity);

                Instantiate(RiseEffect, PlayerEffectSpawnPoint.transform.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(4f);

            state = BattleState.PLAYERTURN;
            StartCoroutine(PlayerTurn());
        }
        else if (PlayerUnit.CurrentAcc > EnemyUnit.CurrentSpeed && PlayNextCounter == 3)
        {
            EnemyAnim.SetBool("EnemySplits", true);

            DialogueText.text = EnemyUnit.UnitName + " uses DOPPELGANGER and duplicates itself!";
            yield return new WaitForSeconds(2f);

            PlayerUnit.Doppel(DoppelAmount);

            //PlayerHUD.StatsText.text = "Accuracy: " + PlayerUnit.CurrentAcc + "%" + "\n" + "Speed: " + PlayerUnit.CurrentSpeed;


            if (PlayerUnit.Damage >= PlayerUnit.MaxDamage)
            {
                DialogueText.text = "Your ACCURACY has lowered \n \n Your ATTACK DAMAGE cannot rise anymore.";
                Instantiate(LowerEffect, PlayerEffectSpawnPointLower.transform.position, Quaternion.identity);

            }
            else if (PlayerUnit.Damage <= 0)
            {
                DialogueText.text = "Your ATTACK DAMAGE cannot lower anymore.";
            }
            else if (PlayerUnit.CurrentAcc >= PlayerUnit.MaxAcc)
            {
                DialogueText.text = "Your ACCURACY cannot rise anymore.";
            }
            else if (PlayerUnit.CurrentAcc <= 0)
            {
                DialogueText.text = "Your ACCURACY cannot lower anymore.\n \n ATTACK DAMAGE has increased!";
                Instantiate(RiseEffect, PlayerEffectSpawnPoint.transform.position, Quaternion.identity);

            }
            else
            {
                DialogueText.text = "Your ACCURACY has lowered \n \n ATTACK DAMAGE has increased!";
                yield return new WaitForSeconds(2f);

                Instantiate(LowerEffect, PlayerEffectSpawnPointLower.transform.position, Quaternion.identity);

                Instantiate(RiseEffect, PlayerEffectSpawnPoint.transform.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(4f);

            state = BattleState.PLAYERTURN;
            StartCoroutine(PlayerTurn());
        }
        else
        {
            PlayerAnim.SetBool("PlayerHit", true);
           

            DialogueText.text = EnemyUnit.UnitName + " ATTACKS!";
            yield return new WaitForSeconds(2f);

            bool isDead = PlayerUnit.TakeDamage(EnemyUnit.Damage);

            PlayerHUD.SetHP(PlayerUnit.CurrentHP);

            DialogueText.text = "You've Been Hit!";

            yield return new WaitForSeconds(2f);

            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                StartCoroutine(PlayerTurn());
            }
        }

    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        //else
        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        //else
        StartCoroutine(PlayerHeal());
    }

    public void OnSmokeButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        //else
        StartCoroutine(PlayerSmoke());
    }

    public void OnDoppelGangButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        //else
        StartCoroutine(PlayerDoppel());
    }

    public void OnRestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnRestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void OnExitButton()
    {
        Application.Quit();
    }

    public void OnNextBattle()
    {
        PlayNextCounter++;
        Debug.Log("cardCount: " + CardCounter);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    public void OnPreviousBattle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void OnEvolution()
    {
        PlayNextCounter = 3;
        Debug.Log("cardCount: " + CardCounter);
        CardCounterText.text = "Gate Cards Won: " + CardCounter;
        if (CardCounter >= 3)
        {
            CardCounter = CardCounter - 3;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
            Debug.Log("cardCount: " + CardCounter);
            CardCounterText.text = "Gate Cards Won: " + CardCounter;
            PlayerCounterCard1.SetActive(false);
            PlayerCounterCard2.SetActive(false);
            PlayerCounterCard3.SetActive(false);
        }
    }

    public void OnDont()
    {
        Debug.Log("cardCount: " + CardCounter);
        CardCounterText.text = "Gate Cards Won: " + CardCounter;
        if (CardCounter >= 1)
        {
            CardCounter--;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Debug.Log("cardCount: " + CardCounter);
            CardCounterText.text = "Gate Cards Won: " + CardCounter;
            PlayerCounterCard1.SetActive(false);
            PlayerCounterCard2.SetActive(false);
            PlayerCounterCard3.SetActive(false);
        }
    }


    public void OnGateCard1()
    {
        GateCardScene.SetActive(false);
        GateCard1.SetActive(false);

        //when activated, the player gets two turns
        ActivateStealCard.SetActive(true);
        

    }


    public void OnGateCard2()
    {
        GateCardScene.SetActive(false);
        GateCard2.SetActive(false);

        //when activated, the player gets two turns
        ActivateStealCard.SetActive(true);
       
    }

    public void OnGateCard3()
    {
        GateCardScene.SetActive(false);
        GateCard3.SetActive(false);

        //when activated, the player and the enemy's HP swtich
        ActivateSwitchCard.SetActive(true);
       
    }

    public void OnSkip()
    {
            isGateCardActivated = true;
            Debug.Log("Gate Card Activated is " + isGateCardActivated);
        
    }

    public void OnSteal()
    {
        GateCardActivated.SetActive(true);
        StartCoroutine(WaitSecs());

        PlayerUnit.CurrentHP += 20;
        EnemyUnit.CurrentHP -= 20;

        PlayerHUD.StatsText.text = "HP: " + PlayerUnit.CurrentHP;
        EnemyHUD.StatsText.text = "HP: " + EnemyUnit.CurrentHP;
        EnemyHUD.SetHP(EnemyUnit.CurrentHP);
        PlayerHUD.SetHP(PlayerUnit.CurrentHP);
    }

    public void EnemySteal()
    {
        EnemyGateCardActivated.SetActive(true);
        StartCoroutine(WaitSecsEnemy());

        PlayerUnit.CurrentHP -= 20;
        EnemyUnit.CurrentHP += 20;

        PlayerHUD.StatsText.text = "HP: " + PlayerUnit.CurrentHP;
        EnemyHUD.StatsText.text = "HP: " + EnemyUnit.CurrentHP;
        EnemyHUD.SetHP(EnemyUnit.CurrentHP);
        PlayerHUD.SetHP(PlayerUnit.CurrentHP);
    }

    public void OnSwitch()
    {
        GateCardActivated.SetActive(true);
        StartCoroutine(WaitSecs());

        PlayerTemp = EnemyUnit.CurrentHP;
        EnemyTemp = PlayerUnit.CurrentHP;

        PlayerUnit.CurrentHP = PlayerTemp;
        EnemyUnit.CurrentHP = EnemyTemp;

        PlayerHUD.StatsText.text = "HP: " + PlayerTemp;
        EnemyHUD.StatsText.text = "HP: " + EnemyTemp;
        EnemyHUD.SetHP(EnemyUnit.CurrentHP);
        PlayerHUD.SetHP(PlayerUnit.CurrentHP);
    }

    public void EnemySwitch()
    {
        EnemyGateCardActivated.SetActive(true);
        StartCoroutine(WaitSecsEnemy());

        PlayerTemp = EnemyUnit.CurrentHP;
        EnemyTemp = PlayerUnit.CurrentHP;

        PlayerUnit.CurrentHP = PlayerTemp;
        EnemyUnit.CurrentHP = EnemyTemp;

        PlayerHUD.StatsText.text = "HP: " + PlayerTemp;
        EnemyHUD.StatsText.text = "HP: " + EnemyTemp;
        EnemyHUD.SetHP(EnemyUnit.CurrentHP);
        PlayerHUD.SetHP(PlayerUnit.CurrentHP);
    }

    public void OnOkay()
    {
        GateCardScene.SetActive(false);

    }

    public void OnEnemyTurnGateCard()
    {
        EnemyGateCardActivated.SetActive(true);
        

        EnemySteal();

        StartCoroutine(WaitSecsEnemy());
        PlayerHUD.StatsText.text = "HP: " + PlayerUnit.CurrentHP;
        EnemyHUD.StatsText.text = "HP: " + EnemyUnit.CurrentHP;
    }

    public void OnBossTurnGateCard()
    {
        EnemyGateCardActivated.SetActive(true);
        

        EnemySwitch();

        StartCoroutine(WaitSecsEnemy());
        PlayerHUD.StatsText.text = "HP: " + PlayerUnit.CurrentHP;
        EnemyHUD.StatsText.text = "HP: " + EnemyUnit.CurrentHP;
    }

    IEnumerator WaitSecs()
    {
        yield return new WaitForSeconds(2f);

        GateCardActivated.SetActive(false);
        ActivateSwitchCard.SetActive(false);
        ActivateStealCard.SetActive(false);
    }

    IEnumerator WaitSecsEnemy()
    {
        yield return new WaitForSeconds(2f);

        EnemyGateCardActivated.SetActive(false);
        
    }
}
