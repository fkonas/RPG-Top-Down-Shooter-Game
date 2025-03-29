using UnityEngine;

public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rifle
}

public enum ShootType
{
    Single,
    Auto
}



[System.Serializable] // Makes class visible in the inspector.

public class Weapon
{
    public WeaponType weaponType;

    [Header("Shooting spesifics")]
    
    public ShootType shootType;
    public int bulletPerShot;
    public float defaultFireRate;
    public float fireRate = 1; // bullets per second
    private float lastShootTime;

    [Header("Burst Fire")]
    public bool burstAvailable;
    public bool burstActive;

    public int burstBulletPerShot;
    public float burstFireRate;
    public float burstFireDelay = .1f;

    [Header("Magazine Details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totalReserveAmmo;

    [Range(1, 3)]
    public float reloadSpeed = 1; // How fast character reload weapon
    [Range(1, 3)]
    public float equipmentSpeed = 1; // How fast character equip weapon
    [Range(2, 12)]
    public float gunDistance = 4;
    [Range(3, 8)]
    public float cameraDistance = 6;

    [Header("Spread ")]
    public float baseSpread = 1;
    public float maximumSpread = 3;
    private float currentSpread = 2;

    public float spreadIncreaseRate = .15f;

    private float lastSpreadUpdateTime;
    private float spreadCooldown = 1;


    #region Spread Methods

    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        float randomizedValue = Random.Range(-currentSpread, currentSpread);

        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);

        return spreadRotation * originalDirection;
    }

    private void UpdateSpread()
    {
        if (Time.time > lastSpreadUpdateTime + spreadCooldown)
        {
            currentSpread = baseSpread;
        }
        else
        {
            IncreaseSpread();
        }

        lastSpreadUpdateTime = Time.time;
    }

    private void IncreaseSpread()
    {
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maximumSpread);
    }

    #endregion

    #region Burst Methods

    public bool BurstActivated()
    {
        if (weaponType == WeaponType.Shotgun)
        {
            burstFireDelay = 0;
            return true;
        }

        return burstActive;
    }

    public void ToggleBurst()
    {
        if (burstAvailable == false)
            return;

        burstActive = !burstActive;

        if (burstActive)
        {
            bulletPerShot = burstBulletPerShot;
            fireRate = burstFireRate;
        }
        else
        {
            bulletPerShot = 1;
            fireRate = defaultFireRate;
        }
    }


    #endregion

    public bool CanShoot() => HaveEnoughBullets() && ReadyToFire();

    private bool ReadyToFire()
    {
        if (Time.time > lastShootTime + 1 / fireRate)
        {
            lastShootTime = Time.time;
            return true;
        }

        return false;
    }

    #region Reload Methods

    public bool CanReload()
    {
        if (bulletsInMagazine == magazineCapacity)
            return false;
        
        if (totalReserveAmmo > 0)
        {
            return true;
        }
        return false;
    }

    public void RefillBullets()
    {
        // totalReserveAmmo += bulletsInMagazine; 
        // this will return add bullets in magazine to total amount of bullets

        int bulletsToReload = magazineCapacity;

        if (bulletsToReload > totalReserveAmmo)
            bulletsToReload = totalReserveAmmo;

        totalReserveAmmo -= bulletsToReload;
        bulletsInMagazine = bulletsToReload;

        if (totalReserveAmmo < 0)
            totalReserveAmmo = 0;

    }

    private bool HaveEnoughBullets() => bulletsInMagazine > 0;

    #endregion
}
