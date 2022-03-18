using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private const int MAX_PORTAL_AMOUNT = 2;
    private int _currentPortalAmount;

    private void Awake()
    {
        _currentPortalAmount = 0;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
