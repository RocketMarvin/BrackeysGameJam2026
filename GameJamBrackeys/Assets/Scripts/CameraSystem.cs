using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private InputReader input;
    [SerializeField] private SerializedArray<Transform> ConstantRoomsArray; // Deze lijst heeft de references naar alle kamers.
    [SerializeField] private GameObject eventSystem;
    [SerializeField] private float cameraSwitchTime = 0.3f;
    [SerializeField] private ParticleSystem TeleportParticles;

    public bool IsZoomedIn = false;

    [HideInInspector] public Transform CurrentRoom;

    private List<Transform> WipedRoomsList = new(); // Deze lijst wordt na elke in en uit zoom leeg gemaakt omdat het gezet wordt op basis van welke kamer je aanklikt.

    void Start()
    {
        if (IsZoomedIn)
        {
            input.SetGameplayActions();
        }
        else
        {
            input.SetUIActions();
        }
    }

    /// <summary>
    /// Open het teleport menu en zoom de camera uit.
    /// </summary>
    private void OnRightClick()
    {
        input.DisableInput();
        StartCoroutine(ZoomCamera(!IsZoomedIn));
    }

    public void SetCurrentRoom(Transform RoomIn)
    {
        CurrentRoom = RoomIn;
        eventSystem.SetActive(false);
        StartCoroutine(ZoomCamera(!IsZoomedIn));
    }

    /// <summary>
    /// Op basis van de huidige kamer en of je in of uitgezoomed bent, zet het je beeld naar voren of naar achteren.
    /// </summary>
    /// <param name="ZoomIn">False is om uit te zoomen en True is om in te zoomen</param>
    /// <returns></returns>
    IEnumerator ZoomCamera(bool ZoomIn)
    {
        foreach (Transform item in CurrentRoom.transform)
        {
            WipedRoomsList.Add(item);
        }
        WipedRoomsList.Add(ConstantRoomsArray[^1]); // Voeg de main camera toe aan de lijst can transforms. (Zorg wel dat je de camera als laatste in de array zet.)

        // Omhoog tellen is uitzoomen omlaag tellen is inzoomen.
        int i = (ZoomIn) ? WipedRoomsList.Count - 1 : 0;
        int j = 0;

        TeleportParticles.Play();

        WipedRoomsList[i].gameObject.SetActive(false);
        WipedRoomsList[ZoomIn ? --i : ++i].gameObject.SetActive(true); // van kamer naar between of andersom.
        TeleportParticles.gameObject.transform.position = ZoomIn ? CurrentRoom.position : transform.position;

        yield return new WaitForSeconds(cameraSwitchTime);
        WipedRoomsList[i].gameObject.SetActive(false);
        WipedRoomsList[ZoomIn ? --i : ++i].gameObject.SetActive(true); // van between naar between2 of andersom.
        TeleportParticles.gameObject.transform.position = ZoomIn ? CurrentRoom.position : transform.position;


        yield return new WaitForSeconds(cameraSwitchTime);
        WipedRoomsList[i].gameObject.SetActive(false);
        WipedRoomsList[ZoomIn ? --i : ++i].gameObject.SetActive(true); // van between2 naar main of andersom.
        TeleportParticles.gameObject.transform.position = ZoomIn ? CurrentRoom.position : transform.position;

        gameObject.transform.position = ZoomIn ? new Vector3(WipedRoomsList[i].transform.position.x, WipedRoomsList[i].transform.position.y, 0) : gameObject.transform.position * 100;
        gameObject.GetComponent<Rigidbody2D>().linearVelocityY = 6;

        WipedRoomsList.Clear();

        if (ZoomIn)
        {
            input.SetGameplayActions();
        }
        else
        {
            eventSystem.SetActive(true);
            input.SetUIActions();
        }

        IsZoomedIn = !IsZoomedIn;
    }

    /// <summary>
    /// Dit is om een array zichtbaar te maken in de inspector.
    /// </summary>
    [Serializable]
    struct SerializedArray<T>
    {
        [SerializeField] private T[] items;
        // Ik snap dit nog niet helemaal maar dit is om indexers aan je eigen structs/classes toe te voegen zodat je instanceVanJeEigenClass[index] kan doen.
        public T this[int index] 
        {
            get => items[index];
            set => items[index] = value;
        }
        public int Length => items.Length;
    }
    private void OnEnable()
    {
        input.RightClickEvent += OnRightClick;
    }

    private void OnDisable()
    {
        input.RightClickEvent -= OnRightClick;
    }
}

