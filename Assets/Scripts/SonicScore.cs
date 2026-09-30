using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SonicScore : MonoBehaviour {
    
    public float timer;
    public string timerToShow;
    private SonicController sonic;
    public SFXmanager sfx;
public int rings;
public float boostGauge;
public float boostgauge_per_ring_fillup;

public bool boostGaugeIncreased = false;
// Declare a global timer object
public float driftTimer;

[Header("UI")]
public Slider boostSlider;
public Slider boostSlider_complete;
public Image boost_ext,boost_begining;
// Rings
public TMP_Text ring_text;
public GameObject lvlUp_ui;
public GameObject driftUp_ui,action_chain_ui;
public TMP_Text action_chain_ui_txt;
public float alive_time_ui = 0.1f;
[SerializeField]private Image deathScreen;
public Sprite boost_begining_img;
    [SerializeField] private Image homingAttack;

private timerRetain time_carrier;

public TMP_Text timertext;

private Sprite defaultspr;
private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        sonic = GetComponent<SonicController>();
        lvlUp_ui.SetActive(false);
        driftUp_ui.SetActive(false);
        action_chain_ui.SetActive(false);
        time_carrier = GameObject.FindWithTag("time_retain").GetComponent<timerRetain>();
        defaultspr = boost_begining.sprite;
    }

	// Use this for initialization
	private void Start()
    {
        // Subscribe to the coin collection event
        RingCollectionManager.Instance.OnRingCollected.AddListener(UpdateRingCount);
        RingCollectionManager.Instance.OnRingCollected.AddListener(BoostLevelUp);
        boost_ext.enabled = false;
        
        
    }
	private void UpdateRingCount(int coinValue)
    {
        sonic.sfx.playActionSound(sonic.sfx.ring);
        rings += coinValue;
		boostGauge += boostgauge_per_ring_fillup;
        Debug.Log("rings: " + rings);

    }
	 private void OnDestroy()
    {
        // Unsubscribe from the coin collection event to avoid memory leaks
        RingCollectionManager.Instance.OnRingCollected.RemoveListener(UpdateRingCount);
	}
	// Update is called once per frame
	void Update () {
		// Limit boost and rings values
        boostGauge = Mathf.Clamp(boostGauge, 0, 6);
        rings = Mathf.Clamp(rings, -1, 300);

         timer += Time.deltaTime;
         timerToShow =  FormatElapsedTime();
         time_carrier.timerToShow = timerToShow;
         timertext.SetText(timerToShow);

        
        UpdateBoostUI();
        UpdateRingsUI();
        DriftLevelUp();
        ActionChainLvlUP();
        HomingRecticleDisplay();

        if(sonic.dead)
        {
            deathScreen.color = new Color (deathScreen.color.r,deathScreen.color.g,deathScreen.color.b, Mathf.Lerp(deathScreen.color.a, 1, 4 * Time.deltaTime) );
            StartCoroutine(RestatScene(1f));
        }
	}  
    IEnumerator RestatScene(float time)
    {
        yield return new WaitForSeconds(time);
        // Get the index of the currently active scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        // Reload the current scene
        SceneManager.LoadScene(currentSceneIndex);
    } 
    void BoostLevelUp(int coin_listn)
    {
        if (rings > 0 && rings % 30 == 0 && !boostGaugeIncreased && coin_listn > 0)
        {
            boostGaugeIncreased = true;
            boostGauge += 1;
            sfx.actionAudioSource.PlayOneShot(sfx.ringUp);
            sfx.boostAudioSource.PlayOneShot(sfx.wooh);
            StartCoroutine(ShowLevelUpUI(lvlUp_ui));
        }
    }
    
    void DriftLevelUp()
    {
        if(sonic.sonic_action == SonicController.Actions.drift && (sonic.drift_direction > 50 && sonic.drift_direction > 0) || (sonic.drift_direction < -50 && sonic.drift_direction < 0))
        {
            driftTimer+= 1 * Time.deltaTime; 
        }
        else
        {
            if(driftTimer > 1)
            {
                boostGauge += 0.5f;

                if(driftTimer > 2)
                {
                    boostGauge += 1;
                }
                sfx.actionAudioSource.PlayOneShot(sfx.boostUp);
                StartCoroutine(ShowLevelUpUI(driftUp_ui));
                
            }
            driftTimer = 0f;
        }
    }

    void ActionChainLvlUP()
    {
        
        if(sonic.actionchain > 0)
        {
            if(sonic.isGrounded ){
                sfx.actionAudioSource.PlayOneShot(sfx.attkUp);
                boostGauge += sonic.actionchain * 0.5f;
                sonic.actionchain = 0;
                StartCoroutine(ShowLevelUpUI(action_chain_ui));
            }
            else
            {
                action_chain_ui_txt_SetNumber(sonic.actionchain);
            }
            action_chain_ui.SetActive(true);
            
            
        }
        
    }
    public IEnumerator ShowLevelUpUI(GameObject ui_obj)
    {
        boostGaugeIncreased = true;
        ui_obj.SetActive(true);

        yield return new WaitForSeconds(0.8f);
        
        ui_obj.SetActive(false);
        boostGaugeIncreased = false;
    }





    private void UpdateBoostUI()
    {
        boostSlider_complete.value = (int)boostGauge;
        boostSlider.value = boostGauge;
        boost_ext.enabled = boostGauge > 3;
         if(boostGauge > 3)
        {
            boost_begining.sprite = boost_begining_img;
        }
        else
        {
            boost_begining.sprite = defaultspr;
        }
    }

    private void UpdateRingsUI()
    {
        ring_text.text = rings.ToString();
    }

     void HomingRecticleDisplay()
    {
        if(!sonic.isGrounded)
        {
            if(sonic.attackObj != null)
            {
                // Convert world position to screen space
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(sonic.attackObj.transform.position);

                // Set the position of the reticle image to match the target position in screen space
                homingAttack.rectTransform.position = screenPosition;
                
                

                // Check if the attack object has changed
                if (sonic.attackObj != sonic.previousAttackObject)
                {
                    sonic.sfx.jumpSFXSource.PlayOneShot(sfx.found_enemy_attack);
                    // Update the previous attack object to match the current one
                    sonic.previousAttackObject = sonic.attackObj;
                }
            }
            
        }
        homingAttack.gameObject.SetActive(sonic.attackObj != null && !sonic.isGrounded);
        
    }


  

    public string FormatElapsedTime()
{
    int minutes = Mathf.FloorToInt(timer / 60);
    int seconds = Mathf.FloorToInt(timer % 60);
    int milliseconds = Mathf.FloorToInt((timer * 100) % 100);

    return string.Format("{0}:{1:00}.{2:00}", minutes, seconds, milliseconds);  
    // Example: "1:23.45"
}

 public void action_chain_ui_txt_SetNumber(int number)
    {
        // Always format the number as two digits (e.g. 5 -> "05")
        string raw = number.ToString("D2");
        string spriteText = "";

        foreach (char c in raw)
        {
            int digit = c - '0';
            spriteText += $"<sprite={digit}>";
        }

        action_chain_ui_txt.text = spriteText;
    }
    
}
