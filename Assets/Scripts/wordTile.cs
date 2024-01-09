//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEditorInternal;
using UnityEngine;
//using TMPro;

public class wordTile : MonoBehaviour
{
    private Rigidbody tileRB;
    public string vocabWord;
    public string wordID;
    public bool firstWord = false;
    public bool secondWord = false;
    private GameManager gm;
    private Vector3 firstWordPos;
    private Vector3 secondWordPos;
    public AudioClip wrong;
    public AudioClip choice;
    private AudioSource tileAudio;

    // Start is called before the first frame update
    void Start()
    {
        tileRB = GetComponent<Rigidbody>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        tileAudio = GetComponent<AudioSource>();

        firstWordPos = new Vector3(-3.5f, 0.9f, -.15f);
        secondWordPos = new Vector3(-1.5f, 0.9f, -.15f);

        tileRB.AddTorque(0f, 0f, Random.Range(-.2f, .2f));
        tileRB.drag = gm.drag;
    }

    // Update is called once per frame
    void Update()
    {
        if (firstWord)
        {
            MoveToCorner(firstWordPos);
        }
        else if (secondWord)
        {
            MoveToCorner(secondWordPos);
        }


        if (transform.position.y < -2f)
        {
            Destroy(gameObject);
        }
    }
    
    void OnMouseDown()
    {
        if (gm.totalFound < 10)
        {
            vocabWord = gameObject.tag.Substring(2);
            wordID = gameObject.tag.Substring(0, 2);

            if (gm.firstWordObject == null)
            {
                firstWord = true;
                tileAudio.PlayOneShot(choice);
                gm.firstWordObject = gameObject;
            }
  
            //in English: if this one isn't the same one again AND if it isn't a different instance of the same object AND if a second object object hasn't been chosen yet (this shouldn't be necessary!!
            //AND if the tags match(minus the ID) AND if one is an image and the other is a picture.
            
            else if (!firstWord && gm.firstWordObject != gameObject && gm.secondWordObject == null && gm.firstWordObject.tag.Substring(2) == vocabWord && gm.firstWordObject.tag.Substring(0, 2) != wordID) 
            {
                //geri, here is how to access the text on the tag. GetComponentInChildren<TextMeshPro>().text;
                secondWord = true;
                tileAudio.PlayOneShot(choice);
                gm.secondWordObject = gameObject;
            }
            else
            {
                tileAudio.PlayOneShot(wrong);
            }
        }
    }
    void MoveToCorner(Vector3 targetPos)
    {
        GetComponent<Rigidbody>().useGravity = false;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, gm.levelSpeed * Time.deltaTime);
        transform.rotation = Quaternion.identity;
        GetComponent<BoxCollider>().enabled = false;
    }
}