using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace PlatformCrafter
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ShootingModule", menuName = "Modules/Shooting")]
    public class ShootingModule : Module
    {
        [SerializeField]
        public GameObject _projectile;

        [Range(0.0f, 50.0f)]
        [SerializeField] private float shootingSpeed;

        private Rigidbody2D rb;

        public override void Initialize(PCModularController controller)
        {
            rb = controller.gameObject.GetComponent<Rigidbody2D>();
        }

        public override void UpdateModule()
        {
            if (!active) return;

            if (Input.GetButtonDown("Fire1")) 
            {
                Vector3 shootDirection = rb.gameObject.transform.right;
                GameObject projectile = Instantiate(_projectile, rb.gameObject.transform.position + shootDirection, Quaternion.identity);
                Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
                projectileRb.velocity = shootDirection * shootingSpeed;
            }
        }
    }
}
