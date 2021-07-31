using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour
{
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;
    public float tiltSmooth = 5;
    public Vector3 startPos;

    public AudioSource tapAudio;
    public AudioSource scoreAudio;
    public AudioSource dieAudio;

    Rigidbody2D rb;
    Quaternion downRotation;
    Quaternion forwardRotation;

    GameManager game;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -90);
        forwardRotation = Quaternion.Euler(0, 0, 35);
        game = GameManager.Instance;
        rb.simulated = false;
    }

    private void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    private void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted()
    {
        rb.velocity = Vector3.zero;
        rb.simulated = true;
    }

    void OnGameOverConfirmed()
    {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }

    private void Update()
    {
        if (game.GameOver) return;
        if (Input.GetMouseButtonDown(0))
        {
            tapAudio.Play();
            transform.rotation = forwardRotation;
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector2.up * tapForce,ForceMode2D.Force);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation,downRotation,tiltSmooth * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ScoreZone") 
        {
            //Register a score event
            OnPlayerScored(); // Event sent to GameManager;
            //play a sound
            scoreAudio.Play();
        }

        if(collision.gameObject.tag == "DeadZone")
        {
            rb.simulated = false;
            //register a dead event
            OnPlayerDied(); //Event sent to GameManager
            //play a sound
            dieAudio.Play();
        }
    }
}
