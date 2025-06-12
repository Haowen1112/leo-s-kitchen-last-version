using System;
using UnityEngine;

public class PlayerController :  KitchenObjectHolder
{
    // Singleton
    public static PlayerController Instance { get; private set; }

    // Events
    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public KitchenObjectHolder selectedCounter;
    }

    // Settings
    [SerializeField] private float speed = 7f;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private Transform _holdPoint;
    [SerializeField] private GameObject[] playerModels;
    [SerializeField] private float footstepInterval = 0.1f;

    // Components
    private Animator animator;
    private const string IS_WALKING = "IsWalking";

    // State
    private bool isWalking;
    private bool canMove;
    private Vector3 lastInteractDir;
    private KitchenObjectHolder selectedHolder;
    private float footstepTimer;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple Player instances detected!");
        }
        Instance = this;

        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        // Initialize player model
        for (int i = 0; i < playerModels.Length; i++)
        {
            playerModels[i].SetActive(false);
        }
        playerModels[PlayerPrefs.GetInt("Player")].SetActive(true);
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
        HandleFootsteps();
        HandleInteractInput();
        HandleAlternateInteractInput();
    }

    private void HandleMovement()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y).normalized;

        // Movement physics
        float playerRadius = 0.7f;
        float playerHeight = 2f;
        float moveDistance = speed * Time.deltaTime;
        canMove = !Physics.CapsuleCast(transform.position,
                                     transform.position + Vector3.up * playerHeight,
                                     playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            // Attempt X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -0.5f || moveDir.x > 0.5f) &&
                     !Physics.CapsuleCast(transform.position,
                                        transform.position + Vector3.up * playerHeight,
                                        playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                // Attempt Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -0.5f || moveDir.z > 0.5f) &&
                         !Physics.CapsuleCast(transform.position,
                                            transform.position + Vector3.up * playerHeight,
                                            playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    moveDir = moveDirZ;
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        // Rotation
        float rotateSpeed = 10f;
        if (moveDir != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
        }

        // Animation
        isWalking = moveDir != Vector3.zero;
        animator.SetBool(IS_WALKING, isWalking);
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit hit, interactDistance, counterLayerMask))
        {
            if (hit.transform.TryGetComponent(out KitchenObjectHolder holder))
            {
                if (holder != selectedHolder)
                {
                    SetSelectedHolder(holder);
                }
            }
            else
            {
                SetSelectedHolder(null);
            }
        }
        else
        {
            SetSelectedHolder(null);
        }
    }

    private void HandleFootsteps()
    {
        if (isWalking)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                footstepTimer = footstepInterval;
                AudioManager.Instance.PlayFootstepSound(transform.position, 1f);
            }
        }
    }

    private void HandleInteractInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!KitchenGameManager.Instance.IsGamePlaying()) return;

            if (selectedHolder != null)
            {
                selectedHolder.Interact(this);
            }
        }
    }

    private void HandleAlternateInteractInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!KitchenGameManager.Instance.IsGamePlaying()) return;

            if (selectedHolder != null)
            {
                selectedHolder.InteractAlternate(this);
            }
        }
    }

    private void SetSelectedHolder(KitchenObjectHolder holder)
    {
        selectedHolder = holder;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = holder
        });
    }

    //#region IKitchenObjectHolder Implementation
    //public Transform GetHoldPoint()
    //{
    //    return _holdPoint;
    //}

    

    //public void HoldObject(KitchenObject kitchenObject)
    //{
    //    _heldKitchenObject = kitchenObject;

    //    if (kitchenObject != null)
    //    {
    //        OnPickedSomething?.Invoke(this, EventArgs.Empty);
    //    }
    //}

    //public KitchenObject GetHeldObject()
    //{
    //    return _heldKitchenObject;
    //}

    //public void ClearHeldObject()
    //{
    //    _heldKitchenObject = null;
    //}

    //public  bool IsHoldingObject()
    //{
    //    return _heldKitchenObject != null;
    //}
    //#endregion

    public bool IsWalking()
    {
        return isWalking;
    }
}