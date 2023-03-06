using UnityEngine;

namespace TrafficGame.Scripts.Level.CarsSpawn.Car
{
    public class CarView : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private CarFrontSafeZone _carFrontSafeZone;

        private Transform _transform;

        public float Speed { get; set; }
        public bool CanMove { get; set; }
        
        public CarPresenter CarPresenter { get; private set; }
        
        public void Awake()
        {
            _transform = transform;
            _carFrontSafeZone.Init(this);
        }

        public void SetPresenter(CarPresenter carPresenter)
        {
            CarPresenter = carPresenter;
        }

        public void SetVelocity(Vector3 velocity)
        {
            _rigidbody.velocity = velocity;
        }
        
        private void FixedUpdate()
        {
            if (CanMove)
            {
                _rigidbody.AddForce(_transform.forward * Speed);
            }
            
            CarPresenter.OnViewTransformUpdate(_rigidbody.position, _rigidbody.rotation);
        }

        private void OnTriggerEnter(Collider other)
        {
            CarPresenter.OnViewTriggerEnter(other);
        }
        
        private void OnTriggerExit(Collider other)
        {
            CarPresenter.OnViewTriggerExit(other);
        }

        private void OnCollisionEnter(Collision collision)
        {
            CarPresenter.OnViewCollisionEnter(collision);
        }
    }
}
