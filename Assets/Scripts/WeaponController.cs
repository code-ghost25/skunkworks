using UnityEngine;
using UnityEngine.InputSystem;

// Attach to the Gun object (or an empty "Weapon" object that's a child of the camera).
public class WeaponController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;            // FPS camera - raycast origin
    [SerializeField] private PlayerLook playerLook;           // for recoil kick, assign the same one on the camera
    [SerializeField] private ParticleSystem muzzleFlash;      // optional, assign later
    [SerializeField] private GameObject impactEffectPrefab;   // optional, spawned at hit point

    [Header("Firing")]
    [SerializeField] private bool isAutomatic = true;
    [SerializeField] private float fireRate = 8f;   // rounds per second
    [SerializeField] private float range = 100f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private LayerMask hitMask = ~0; // ~0 = everything; narrow this once you have layers set up

    [Header("Ammo")]
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private float reloadTime = 1.6f;

    [Header("Recoil")]
    [SerializeField] private float recoilPerShot = 1.2f;

    private int currentAmmo;
    private float nextFireTime;
    private bool isReloading;

    private void Awake()
    {
        currentAmmo = magazineSize;
    }

    private void Update()
    {
        HandleReloadInput();
        HandleFireInput();
    }

    private void HandleFireInput()
    {
        if (isReloading) return;

        bool wantsToFire = isAutomatic
            ? Mouse.current.leftButton.isPressed
            : Mouse.current.leftButton.wasPressedThisFrame;

        if (!wantsToFire || Time.time < nextFireTime) return;

        if (currentAmmo <= 0)
        {
            StartReload();
            return;
        }

        Fire();
    }

    private void Fire()
    {
        nextFireTime = Time.time + 1f / fireRate;
        currentAmmo--;

        if (muzzleFlash != null) muzzleFlash.Play();
        if (playerLook != null) playerLook.AddRecoil(recoilPerShot);

        // Raycast from the exact center of the screen, not the gun's muzzle -
        // this is what makes the crosshair "honest" (what you see is what you hit).
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            if (impactEffectPrefab != null)
            {
                Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            }

            IDamageable target = hit.collider.GetComponentInParent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
    }

    private void HandleReloadInput()
    {
        if (!isReloading && Keyboard.current.rKey.wasPressedThisFrame && currentAmmo < magazineSize)
        {
            StartReload();
        }
    }

    private void StartReload()
    {
        isReloading = true;
        Invoke(nameof(FinishReload), reloadTime);
    }

    private void FinishReload()
    {
        currentAmmo = magazineSize;
        isReloading = false;
    }

    // Simple accessors if you want to hook up ammo UI later.
    public int CurrentAmmo => currentAmmo;
    public int MagazineSize => magazineSize;
    public bool IsReloading => isReloading;
}