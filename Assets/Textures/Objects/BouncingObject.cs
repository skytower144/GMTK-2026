using System.Collections;
using UnityEngine;

namespace TomatoPunch
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BouncingObject : MonoBehaviour
    {
        protected const float BOUNCINESS = 0.5f;
        protected const float BOUNCE_SLIDE_MULTIPLIER = 0.8f;
        protected const int MAX_BOUNCE_COUNT = 2;
        
        protected const float BLINK_INTERVAL = 0.06f;
        protected const int MAX_BLINK_COUNT = 8;

        protected const float FRICTION = 4f;
        protected const float FAKE_GRAVITY = -25f;
        protected const float MAX_LIFE_TIME = 3f;

        protected SpriteRenderer spriteRenderer;
        private Vector2 horizontalVelocity;
        private Vector2 currentPosition;

        private float heightY = 0f;
        private float verticalForce;
        protected int remainingBounceCount;

        private float lifeTime;
        protected bool isDestroyed;
        private bool startBouncing = false;

        private readonly WaitForSeconds blinkWait = new(BLINK_INTERVAL);

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Init(float slideSpeed, float initialUpwardForce)
        {
            Vector2 slideDir = Random.insideUnitCircle.normalized;
            horizontalVelocity = slideDir * slideSpeed;

            verticalForce = initialUpwardForce;

            remainingBounceCount = MAX_BOUNCE_COUNT;
            lifeTime = MAX_LIFE_TIME;

            currentPosition = transform.position;
            startBouncing = true;
        }

        void Update()
        {
            if (isDestroyed)
                return;
            
            if (!startBouncing)
                return;

            SlideAndBounce();
            CheckDestroyTimer();
        }

        private void SlideAndBounce()
        {
            currentPosition += horizontalVelocity * Time.deltaTime;
            horizontalVelocity = Vector2.MoveTowards(horizontalVelocity, Vector2.zero, FRICTION * Time.deltaTime);

            verticalForce += FAKE_GRAVITY * Time.deltaTime;
            heightY += verticalForce * Time.deltaTime;

            if (heightY <= 0f)
            {
                heightY = 0f;

                if (remainingBounceCount > 0)
                {
                    --remainingBounceCount;
                    verticalForce = -verticalForce * BOUNCINESS;
                    horizontalVelocity *= BOUNCE_SLIDE_MULTIPLIER;
                }
                else
                {
                    verticalForce = 0;
                    horizontalVelocity = Vector2.zero;
                }
            }

            transform.position = new Vector3(currentPosition.x, currentPosition.y + heightY, transform.position.z);
        }

        private void CheckDestroyTimer()
        {
            if (isDestroyed)
                return;
            
            lifeTime -= Time.deltaTime;

            if (lifeTime <= 0f)
            {
                isDestroyed = true;
                StartCoroutine(BlinkAndDestroy());
            }
        }

        private IEnumerator BlinkAndDestroy()
        {
            for (int i = 0; i < MAX_BLINK_COUNT; i++)
            {
                spriteRenderer.enabled = true;
                yield return blinkWait;

                spriteRenderer.enabled = false;
                yield return blinkWait;
            }

            Destroy(gameObject);
        }
    }
}