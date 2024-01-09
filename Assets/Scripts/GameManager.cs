//using System.Collections;
using System.Collections.Generic;
//using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class GameManager : MonoBehaviour
{
    public List<GameObject> vocab;
    public List<string> completeList;
    public GameObject firstWordObject;
    public GameObject secondWordObject;

    //GUI stuff
    public TextMeshProUGUI wordsCompleted;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI stopwatch;
    public Button clearButton;
    public Button restartButton;
    public GameObject allLevelButtons;

    public ParticleSystem yay;
    public AudioClip tada;
    public AudioClip applause;
    private AudioSource gameAudio;
    public int totalFound;
    private float beginTime;
    private Vector3 startPosCompleted = new Vector3(-6.7f, 5.6f, 0.1f);

    //these ones get set in each level
    private float repeatRate;
    public int drag;
    private int endSpawnIndex;
    public float levelSpeed;

    void Start()
    {
        totalFound = 0;
        allLevelButtons = GameObject.Find("allLevelButtons");
    }

    // Update is called once per frame
    void Update()
    {
        stopwatch.text = (Time.realtimeSinceStartup - beginTime).ToString("#.#");
        if (firstWordObject)
        {
            clearButton.gameObject.SetActive(true);
        }

        wordsCompleted.text = "completed: " + totalFound + "/10";

        //when we find a pair
        if (firstWordObject && secondWordObject)
        {
            clearButton.gameObject.SetActive(false);
            if (secondWordObject.transform.position.x == -1.5f)
            {
                gameAudio.PlayOneShot(tada);
                yay.Play();

                //remove correct pairs from List so they don't keep spawning
                int i = 0;
                while (i < vocab.Count)
                {
                    if (vocab[i].CompareTag(firstWordObject.tag))
                    {
                        //firstWordObject = null;
                        vocab.Remove(vocab[i]);
                        if (i != 0)
                        {
                            i--;
                            continue;
                        }
                        //once an item is removed, the next item gets skipped if I don't decrement i
                    }
                    if (vocab[i].CompareTag(secondWordObject.tag))
                    {
                        //secondWordObject = null;
                        vocab.Remove(vocab[i]);
                        if (i != 0)
                        {
                            i--;
                            continue;
                        }
                    }
                    i++;
                }
                // puts image in "completed" area, unless already there, destroys word tile, resets.
                if (firstWordObject.tag.Substring(0, 2) == "01" && !completeList.Contains(firstWordObject.tag.Substring(2, firstWordObject.tag.Length - 2)))
                {
                    totalFound++;
                    firstWordObject.GetComponent<wordTile>().firstWord = false;
                    firstWordObject.transform.position = startPosCompleted + new Vector3((totalFound + 1) % 2, -1f * (float)((int)((totalFound + 1) / 2f)), 0f);
                    firstWordObject.transform.rotation = Quaternion.identity;
                    firstWordObject.transform.localScale = firstWordObject.transform.localScale * .5f;
                    firstWordObject.GetComponent<Rigidbody>().freezeRotation = true;
                    completeList.Add(firstWordObject.tag.Substring(2, firstWordObject.tag.Length - 2));
                    firstWordObject = null;
                    Destroy(secondWordObject);
                }

                else if (secondWordObject.tag.Substring(0, 2) == "01" && !completeList.Contains(secondWordObject.tag.Substring(2, secondWordObject.tag.Length - 2)))
                {
                    totalFound++;
                    secondWordObject.GetComponent<wordTile>().secondWord = false;
                    secondWordObject.transform.position = startPosCompleted + new Vector3((totalFound + 1) % 2, -1f * (float)((int)((totalFound + 1) / 2f)), 0f);
                    secondWordObject.transform.rotation = Quaternion.identity;
                    secondWordObject.transform.localScale = secondWordObject.transform.localScale * .5f;
                    secondWordObject.GetComponent<Rigidbody>().freezeRotation = true;
                    completeList.Add(firstWordObject.tag.Substring(2, firstWordObject.tag.Length - 2));
                    secondWordObject = null;
                    Destroy(firstWordObject);
                }

                else
                {
                    Destroy(firstWordObject);
                    Destroy(secondWordObject);
                }
            }
        }
        if (totalFound == 10)
        {
            gameAudio.PlayOneShot(applause);
            GameObject.Find("paper").GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            gameOverText.text += stopwatch.text;
            gameOverText.gameObject.SetActive(true);
            stopwatch.gameObject.SetActive(false);
            wordsCompleted.gameObject.SetActive(false);
            totalFound += 1;
            restartButton.gameObject.SetActive(true);
            CancelInvoke("SpawnWords");
        }
    }

    public void ClearChoice()
    {
        Destroy(firstWordObject);
    }

    public void StartLevel1()
    {
        beginTime = Time.realtimeSinceStartup;
        totalFound = 0;
        stopwatch.gameObject.SetActive(true);
        allLevelButtons.SetActive(false);
        repeatRate = 1.2f;
        drag = 16;
        endSpawnIndex = 3;
        levelSpeed = 3.2f;
        InvokeRepeating("SpawnWords", 0, repeatRate);
        gameAudio = GetComponent<AudioSource>();
    }

    public void StartLevel2()
    {
        beginTime = Time.realtimeSinceStartup;
        totalFound = 0;
        stopwatch.gameObject.SetActive(true);
        allLevelButtons.SetActive(false);
        repeatRate = 0.8f;
        drag = 12;
        endSpawnIndex = 5;
        levelSpeed = 4f;
        InvokeRepeating("SpawnWords", 0, repeatRate);
        gameAudio = GetComponent<AudioSource>();
    }

    public void StartLevel3()
    {
        beginTime = Time.realtimeSinceStartup;
        totalFound = 0;
        stopwatch.gameObject.SetActive(true);
        allLevelButtons.SetActive(false);
        repeatRate = 0.5f;
        drag = 8;
        endSpawnIndex = 7;
        levelSpeed = 5.2f;
        InvokeRepeating("SpawnWords", 0, repeatRate);
        gameAudio = GetComponent<AudioSource>();
    }

    public void StartAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    void SpawnWords()
    {
        if (totalFound < 10)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-4f, 4f), 7f, Random.Range(-.12f, -.16f));
            //if the vocab List is empty, don't instantiate 
            if (vocab.Count >= endSpawnIndex * 3)
            {
                int index = Random.Range(0, endSpawnIndex * 3);
                Instantiate(vocab[index], spawnPos, Quaternion.identity);
            }
            else if (vocab.Count < endSpawnIndex * 3)
            {
                int index = Random.Range(0, vocab.Count);
                Instantiate(vocab[index], spawnPos, Quaternion.identity);
            }
        }
    }
}
