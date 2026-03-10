using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private InputReader input;
    [SerializeField] private GameObject eventSystem;
    [SerializeField] private float cameraSwitchTime = 0.3f;
    [SerializeField] private ParticleSystem TeleportParticles;
    [SerializeField] private Transform MainCamera;

    [SerializeField] private ItemPickUpManager itemPickUpManager;

    private bool isZooming = false; // voorkomt dubbele coroutines
    bool IsZoomedIn = true;
    [SerializeField] private Transform CurrentRoom;
    [SerializeField] private Transform OceanRoom;
    [SerializeField] private Transform AlienRoom;
    [SerializeField] private bool oceanRoomHeated = false;
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
        if (isZooming) return;

        if (itemPickUpManager != null && itemPickUpManager.equippedTeleporter && CurrentRoom != null)
        {
            UpdateOceanRoomStatus();

            if (CurrentRoom == OceanRoom && oceanRoomHeated && !itemPickUpManager.equippedHeatResist)
            {
                Debug.Log("De OceanRoom is te heet! Je hebt een hitteItem nodig!");
                return;
            }

            input.DisableInput();
            StartCoroutine(ZoomCamera(!IsZoomedIn));
        }
        else
        {
            Debug.Log("Teleport niet toegestaan of CurrentRoom nog niet gezet");
        }
    }

    private void UpdateOceanRoomStatus()
    {
        if (itemPickUpManager != null && itemPickUpManager.wrenchGrabbed)
        {
            // Alleen heated als we de OceanRoom nog niet betreden
            if (CurrentRoom != OceanRoom)
                oceanRoomHeated = true;
            else
                oceanRoomHeated = false; // in de OceanRoom mag je altijd uit
        }
        else
        {
            oceanRoomHeated = false;
        }
    }

    public void SetCurrentRoom(Transform RoomIn)
    {
        if (isZooming) return;

        UpdateOceanRoomStatus();

        if (itemPickUpManager != null && itemPickUpManager.alienActive && RoomIn == AlienRoom)
        {
            Debug.Log("De alien blokkeert de kamer!");
            return;
        }

        if (RoomIn == OceanRoom)
        {
            if (!itemPickUpManager.equippedDiveSuit)
            {
                Debug.Log("Je hebt een DiveSuit nodig om de OceanRoom te betreden!");
                return;
            }

            if (oceanRoomHeated && !itemPickUpManager.equippedHeatResist)
            {
                Debug.Log("De OceanRoom is te heet! Je hebt een hitteItem nodig!");
                return;
            }
        }

        CurrentRoom = RoomIn;
        StartCoroutine(ZoomCamera(!IsZoomedIn));
    }

    /// <summary>
    /// Op basis van de huidige kamer en of je in of uitgezoomed bent, zet het je beeld naar voren of naar achteren.
    /// </summary>
    /// <param name="ZoomIn">False is om uit te zoomen en True is om in te zoomen</param>
    /// <returns></returns>
    IEnumerator ZoomCamera(bool ZoomIn)
    {
        isZooming = true;

        foreach (Transform item in CurrentRoom.transform)
        {
            WipedRoomsList.Add(item);
        }
        WipedRoomsList.Add(MainCamera.transform);

        int i = (ZoomIn) ? WipedRoomsList.Count - 1 : 0;

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

        yield return new WaitForSeconds(cameraSwitchTime * 2);

        if (ZoomIn)
            input.SetGameplayActions();
        else
        {
            eventSystem.SetActive(true);
            input.SetUIActions();
        }

        IsZoomedIn = ZoomIn;

        UpdateAlienDoorInput();

        isZooming = false;
    }

    private void OnEnable()
    {
        input.RightClickEvent += OnRightClick;
    }

    private void OnDisable()
    {
        input.RightClickEvent -= OnRightClick;
    }

    public void SetAlienDoorState(bool state)
    {
        itemPickUpManager.inAlienDoor = state;
        UpdateAlienDoorInput();
    }
    void UpdateAlienDoorInput()
    {
        if (itemPickUpManager.equippedTranslator && itemPickUpManager.inAlienDoor && CurrentRoom == AlienRoom)
        {
            input.SetUIAndGameplayActions();
            Debug.Log("Alien deur UI + movement actief");
        }
        else
        {
            input.SetGameplayActions();
        }
    }
}

