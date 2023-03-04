using System;
using System.Collections.Generic;
using Input;
using UnityEngine;

namespace Controllers {
    public class HumanoidLandController : MonoBehaviour {
        private const double TOLERANCE = 0.0005f;

        public Transform cameraFollow;
        private Vector3 playerLookInput;
        private Vector3 previousPlayerLookInput;
        private float cameraPitch;
        [ SerializeField ]
        private CameraController cameraController;
        [ SerializeField ]
        private float playerLookInputLerpTime = 0.35f;
        private Rigidbody rb;
        private CapsuleCollider cc;
        [ SerializeField ]
        private HumanoidLandInput input;
        [ SerializeField ]
        private Vector3 playerMoveInput;

        [ Header("Movement") ]
        [ SerializeField ]
        private float movementMultiplier = 30f;
        [ SerializeField ]
        private float notGroundedMovementMultiplier = 0.25f;
        [ SerializeField ]
        private float rotationSpeedMultiplier = 45f;
        [ SerializeField ]
        private float pitchSpeedMultiplier = 45f;
        [ SerializeField ]
        private float crouchSpeedMultiplier = 0.5f;
        [ SerializeField ]
        private float lookUpLimit = 60f;
        [ SerializeField ]
        private float lookDownLimit = -40f;
        [ SerializeField ]
        private float runMultiplier = 2.5f;

        [ Header("Ground Check") ]
        [ SerializeField ]
        private bool playerIsGrounded;
        [ SerializeField ]
        [ Range(0f, 1.8f) ]
        private float groundCheckRadiusMultiplier = 0.9f;
        [ SerializeField ]
        [ Range(-0.95f, 1.05f) ]
        private float groundCheckDistanceTolerance = 0.05f;
        [ SerializeField ]
        private float playerCenterToGroundDistance;
        private RaycastHit groundCheckHit;

        [ Header("Gravity") ]
        [ SerializeField ]
        private float gravityFallCurrent;
        [ SerializeField ]
        private float gravityFallMin;
        [ SerializeField ]
        private float gravityFallIncrementTime = 0.05f;
        [ SerializeField ]
        private float playerFallTimer;
        [ SerializeField ]
        private float gravityGrounded = -1f;
        [ SerializeField ]
        private float maxSlopeAngle = 47.5f;

        [ Header("Stairs") ]
        [ SerializeField ]
        [ Range(0f, 1f) ]
        private float maxStepHeight = 0.5f;
        [ SerializeField ]
        [ Range(0f, 1f) ]
        private float minStepDepth = 0.3f;
        [ SerializeField ]
        private float stairHeightPaddingMultiplier = 1.5f;
        [ SerializeField ]
        private bool isFirstStep = true;
        [ SerializeField ]
        private float firstStepVelocityDistanceMultiplier = 0.1f;
        [ SerializeField ]
        private bool playerIsAscendingStairs;
        [ SerializeField ]
        private bool playerIsDescendingStairs;
        [ SerializeField ]
        private float ascendingStairsMovementMultiplier = 0.7f;
        [ SerializeField ]
        private float descendingStairsMovementMultiplier = 1.25f;
        [ SerializeField ]
        private float maximumAngleOfApproachToAscend = 45f;
        private float playerHalfHeightToGround;
        private float maxAscendRayDistance;
        private float maxDescendRayDistance;
        private int numberOfStepDetectRays;
        private float rayIncrementAmount;

        [ Header("Jumping") ]
        [ SerializeField ]
        private float initialJumpForceMultiplier = 750f;
        [ SerializeField ]
        private float continualJumpForceMultiplier = 0.1f;
        [ SerializeField ]
        private float jumpTime = 0.175f;
        [ SerializeField ]
        private float jumpTimeCounter;
        [ SerializeField ]
        private float coyoteTime = 0.15f;
        [ SerializeField ]
        private float coyoteTimeCounter;
        [ SerializeField ]
        private float jumpBufferTime = 0.2f;
        [ SerializeField ]
        private float jumpBufferTimeCounter;
        [ SerializeField ]
        private bool isJumping;
        [ SerializeField ]
        private bool jumpPressedLastFrame;
        [ SerializeField ]
        private float jumpReactionForceMultiplier = 3f;
        private RaycastHit lastGroundCheckHit;
        private Vector3 playerMoveInputAtLastGroundCheckHit;

        [ Header("Crouching") ]
        [ SerializeField ]
        private bool playerIsCrouching;
        [ SerializeField ]
        [ Range(0f, 1.8f) ]
        private float headCheckRadiusMultiplier;
        [ SerializeField ]
        private float crouchTimeMultiplier = 10f;
        [ SerializeField ]
        private float playerCrouchedHeightTolerance = 0.05f;
        private float crouchAmount = 1f;
        private float playerFullHeight;
        private float playerCrouchedHeight;
        private Vector3 playerCenterPoint = Vector3.zero;

        private void Awake() {
            rb = GetComponent<Rigidbody>();
            cc = GetComponent<CapsuleCollider>();

            maxAscendRayDistance = maxStepHeight / Mathf.Cos(maximumAngleOfApproachToAscend * Mathf.Deg2Rad);
            maxDescendRayDistance = maxStepHeight / Mathf.Cos(80f * Mathf.Deg2Rad);

            numberOfStepDetectRays = Mathf.RoundToInt(maxStepHeight * 100f * 0.5f + 1f);
            rayIncrementAmount = maxStepHeight / numberOfStepDetectRays;

            playerFullHeight = cc.height;
            playerCrouchedHeight = playerFullHeight - crouchAmount;
        }

        private void FixedUpdate() {
            if (!cameraController.usingOrbitalCamera) {
                playerLookInput = GetLookInput();
                PlayerLook();
                PitchCamera();
            }
            playerMoveInput = GetMoveInput();

            PlayerVariables();

            playerIsGrounded = PlayerGroundCheck();

            playerMoveInput = PlayerMove();

            // todo redo stairs in another method
            playerMoveInput = PlayerStairs();
            // todo redo slopes in another method
            playerMoveInput = PlayerSlope();

            playerMoveInput = PlayerCrouch();
            playerMoveInput = PlayerRun();

            PlayerInfoCapture();

            playerMoveInput.y = PlayerFallGravity();
            playerMoveInput.y = PlayerJump();

            Debug.DrawRay(playerCenterPoint, rb.transform.TransformDirection(playerMoveInput), Color.red, 0.5f);

            rb.AddRelativeForce(playerMoveInput * rb.mass, ForceMode.Force);
        }

        private Vector3 GetLookInput() {
            previousPlayerLookInput = playerLookInput;
            playerLookInput = new Vector3(input.look.x, input.invertMouse ? -input.look.y : input.look.y, 0f);
            return Vector3.Lerp(previousPlayerLookInput, playerLookInput, playerLookInputLerpTime);
        }

        private void PlayerLook() {
            rb.rotation = Quaternion.Euler(
                                           0f,
                                           rb.rotation.eulerAngles.y + playerLookInput.x * rotationSpeedMultiplier,
                                           0f);
        }

        private void PitchCamera() {
            var rotationValues = cameraFollow.rotation.eulerAngles;
            cameraPitch += playerLookInput.y * pitchSpeedMultiplier;
            cameraPitch = Mathf.Clamp(cameraPitch, lookDownLimit, lookUpLimit);
            cameraFollow.rotation = Quaternion.Euler(cameraPitch, rotationValues.y, rotationValues.z);
        }

        private Vector3 GetMoveInput() {
            return new Vector3(input.move.x, 0f, input.move.y);
        }

        private void PlayerVariables() {
            playerCenterPoint = rb.position + cc.center;
        }

        private bool PlayerGroundCheck() {
            var sphereCastRadius = cc.radius * groundCheckRadiusMultiplier;
            Physics.SphereCast(playerCenterPoint, sphereCastRadius, Vector3.down, out groundCheckHit);
            playerCenterToGroundDistance = groundCheckHit.distance + sphereCastRadius;
            return playerCenterToGroundDistance >= cc.bounds.extents.y - groundCheckDistanceTolerance
                && playerCenterToGroundDistance <= cc.bounds.extents.y + groundCheckDistanceTolerance;
        }

        private Vector3 PlayerMove() {
            return playerIsGrounded
                       ? playerMoveInput * movementMultiplier
                       : playerMoveInput * movementMultiplier * notGroundedMovementMultiplier;
        }

        private Vector3 PlayerStairs() {
            var calculatedStepInput = playerMoveInput;
            playerHalfHeightToGround = cc.bounds.extents.y;
            if (playerCenterToGroundDistance < cc.bounds.extents.y) {
                playerHalfHeightToGround = playerCenterToGroundDistance;
            }
            calculatedStepInput = AscendStairs(calculatedStepInput);
            if (!playerIsAscendingStairs) {
                calculatedStepInput = DescendStairs(calculatedStepInput);
            }
            return calculatedStepInput;
        }

        private Vector3 AscendStairs(Vector3 calculatedStepInput) {
            if (input.moveIsPressed) {
                var calculatedVelDistance
                    = isFirstStep
                          ? rb.velocity.magnitude * firstStepVelocityDistanceMultiplier + cc.radius
                          : cc.radius;
                var ray = 0f;
                var raysThatHit = new List<RaycastHit>();
                for (var x = 0; x <= numberOfStepDetectRays; x++, ray += rayIncrementAmount) {
                    var rayLower = new Vector3(playerCenterPoint.x,
                                               playerCenterPoint.y - playerHalfHeightToGround + ray,
                                               playerCenterPoint.z);
                    if (Physics.Raycast(rayLower,
                                        rb.transform.TransformDirection(playerMoveInput),
                                        out var hitLower,
                                        calculatedVelDistance + maxAscendRayDistance)) {
                        var stairsSlopeAngle = Vector3.Angle(hitLower.normal, rb.transform.up);
                        if (Math.Abs(stairsSlopeAngle - 90f) < TOLERANCE) {
                            raysThatHit.Add(hitLower);
                        }
                    }
                }
                if (raysThatHit.Count > 0) {
                    var rayUpper = new Vector3(playerCenterPoint.x,
                                               playerCenterPoint.y
                                             - playerHalfHeightToGround
                                             + maxStepHeight
                                             + rayIncrementAmount,
                                               playerCenterPoint.z);
                    Physics.Raycast(rayUpper,
                                    rb.transform.TransformDirection(playerMoveInput),
                                    out var hitUpper,
                                    calculatedVelDistance + maxAscendRayDistance * 2f);
                    if (!hitUpper.collider || hitUpper.distance - raysThatHit[0].distance > minStepDepth) {
                        if (Vector3.Angle(raysThatHit[0].normal, rb.transform.TransformDirection(-playerMoveInput))
                         <= maximumAngleOfApproachToAscend) {
                            Debug.DrawRay(rayUpper, rb.transform.TransformDirection(playerMoveInput), Color.yellow, 5f);
                            playerIsAscendingStairs = true;
                            var playerRelX = Vector3.Cross(playerMoveInput, Vector3.up);
                            if (isFirstStep) {
                                calculatedStepInput = Quaternion.AngleAxis(45f, playerRelX) * calculatedStepInput;
                                isFirstStep = false;
                            } else {
                                var stairHeight = raysThatHit.Count * rayIncrementAmount * stairHeightPaddingMultiplier;
                                var avgDistance = 0f;
                                foreach (var hit in raysThatHit) {
                                    avgDistance += hit.distance;
                                }
                                avgDistance /= raysThatHit.Count;
                                var tanAngle = Mathf.Atan2(stairHeight, avgDistance) * Mathf.Rad2Deg;
                                calculatedStepInput = Quaternion.AngleAxis(tanAngle, playerRelX) * calculatedStepInput;
                                calculatedStepInput *= ascendingStairsMovementMultiplier;
                            }
                        } else {
                            // more than 45degrees angle of approach
                            playerIsAscendingStairs = false;
                            isFirstStep = true;
                        }
                    } else {
                        // top ray hit something
                        playerIsAscendingStairs = false;
                        isFirstStep = true;
                    }
                } else {
                    // no rays hit
                    playerIsAscendingStairs = false;
                    isFirstStep = true;
                }
            } else {
                // move is not pressed
                playerIsAscendingStairs = false;
                isFirstStep = true;
            }
            return calculatedStepInput;
        }

        private Vector3 DescendStairs(Vector3 calculatedStepInput) {
            if (input.moveIsPressed) {
                var ray = 0f;
                var raysThatHit = new List<RaycastHit>();
                for (var x = 1; x <= numberOfStepDetectRays; x++, ray += rayIncrementAmount) {
                    var rayLower = new Vector3(playerCenterPoint.x,
                                               playerCenterPoint.y - playerHalfHeightToGround + ray,
                                               playerCenterPoint.z);
                    if (Physics.Raycast(rayLower,
                                        rb.transform.TransformDirection(-playerMoveInput),
                                        out var hitLower,
                                        cc.radius + maxDescendRayDistance)) {
                        var stairSlopeAngle = Vector3.Angle(hitLower.normal, rb.transform.up);
                        if (Math.Abs(stairSlopeAngle - 90f) < TOLERANCE) {
                            raysThatHit.Add(hitLower);
                        }
                    }
                }
                if (raysThatHit.Count > 0) {
                    var rayUpper = new Vector3(playerCenterPoint.x,
                                               playerCenterPoint.y
                                             - playerHalfHeightToGround
                                             + maxStepHeight
                                             + rayIncrementAmount,
                                               playerCenterPoint.z);
                    Physics.Raycast(rayUpper,
                                    rb.transform.TransformDirection(-playerMoveInput),
                                    out var hitUpper,
                                    cc.radius + maxDescendRayDistance * 2f);
                    if (hitUpper.collider || hitUpper.distance - raysThatHit[0].distance > minStepDepth) {
                        if (!playerIsGrounded && hitUpper.distance < cc.radius + maxDescendRayDistance * 2f) {
                            Debug.DrawRay(rayUpper,
                                          rb.transform.TransformDirection(-playerMoveInput),
                                          Color.yellow,
                                          5f);
                            playerIsDescendingStairs = true;
                            var playerRelX = Vector3.Cross(playerMoveInput, Vector3.up);
                            var stairHeight = raysThatHit.Count * rayIncrementAmount * stairHeightPaddingMultiplier;
                            var avgDistance = 0f;
                            foreach (var hit in raysThatHit) {
                                avgDistance += hit.distance;
                            }
                            avgDistance /= raysThatHit.Count;
                            var tanAngle = Mathf.Atan2(stairHeight, avgDistance) * Mathf.Rad2Deg;
                            calculatedStepInput
                                = Quaternion.AngleAxis(tanAngle - 90f, playerRelX) * calculatedStepInput;
                            calculatedStepInput *= descendingStairsMovementMultiplier;
                        } else {
                            // more than 45degrees angle of approach
                            playerIsDescendingStairs = false;
                        }
                    } else {
                        // top ray hit something
                        playerIsDescendingStairs = false;
                    }
                } else {
                    // no rays hit
                    playerIsDescendingStairs = false;
                }
            } else {
                // move is not pressed
                playerIsDescendingStairs = false;
            }
            return calculatedStepInput;
        }

        private Vector3 PlayerSlope() {
            var calculatedPlayerMovement = playerMoveInput;
            if (playerIsGrounded && !playerIsAscendingStairs && !playerIsDescendingStairs) {
                var localGroundCheckHitNormal = rb.transform.InverseTransformDirection(groundCheckHit.normal);
                var groundSlopeAngle = Vector3.Angle(localGroundCheckHitNormal, rb.transform.up);
                if (groundSlopeAngle == 0) {
                    if (input.moveIsPressed) {
                        var rayCalculatedRayHeight
                            = playerCenterPoint.y - playerCenterToGroundDistance + groundCheckDistanceTolerance;
                        var rayOrigin = new Vector3(playerCenterPoint.x, rayCalculatedRayHeight, playerCenterPoint.z);
                        if (Physics.Raycast(rayOrigin,
                                            rb.transform.TransformDirection(calculatedPlayerMovement),
                                            out var hit,
                                            0.75f)) {
                            if (Vector3.Angle(hit.normal, rb.transform.up) > maxSlopeAngle) {
                                calculatedPlayerMovement.y = -movementMultiplier;
                            }
                        }
                        #if UNITY_EDITOR
                        Debug.DrawRay(rayOrigin,
                                      rb.transform.TransformDirection(calculatedPlayerMovement),
                                      Color.green,
                                      1f);
                        #endif
                    }
                    if (calculatedPlayerMovement.y == 0f) {
                        calculatedPlayerMovement.y = gravityGrounded;
                    }
                } else {
                    var slopeAngleRotation = Quaternion.FromToRotation(rb.transform.up, localGroundCheckHitNormal);
                    calculatedPlayerMovement = slopeAngleRotation * calculatedPlayerMovement;
                    var relativeSlopeAngle = Vector3.Angle(calculatedPlayerMovement, rb.transform.up) - 90f;
                    calculatedPlayerMovement += calculatedPlayerMovement * (relativeSlopeAngle / 90f);
                    if (groundSlopeAngle < maxSlopeAngle) {
                        if (input.moveIsPressed) {
                            calculatedPlayerMovement.y += gravityGrounded;
                        }
                    } else {
                        var calculatedSlopeGravity = groundSlopeAngle * -0.2f;
                        if (calculatedSlopeGravity < calculatedPlayerMovement.y) {
                            calculatedPlayerMovement.y = calculatedSlopeGravity;
                        }
                    }
                }
                #if UNITY_EDITOR
                Debug.DrawRay(playerCenterPoint,
                              rb.transform.TransformDirection(calculatedPlayerMovement),
                              Color.red,
                              0.5f);
                #endif
            }
            return calculatedPlayerMovement;
        }

        private Vector3 PlayerCrouch() {
            var calculatedPlayerCrouchSpeed = playerMoveInput;
            if (input.crouch) {
                Crouch();
            } else if (playerIsCrouching) {
                UnCrouch();
            }
            if (playerIsCrouching) {
                calculatedPlayerCrouchSpeed *= crouchSpeedMultiplier;
            }
            return calculatedPlayerCrouchSpeed;
        }

        private void Crouch() {
            if (cc.height >= playerCrouchedHeight + playerCrouchedHeightTolerance) {
                var time = Time.fixedDeltaTime * crouchTimeMultiplier;
                var amount = Mathf.Lerp(0, crouchAmount, time);
                cc.height -= amount;
                cc.center = new Vector3(cc.center.x, cc.center.y + (amount * 0.5f), cc.center.z);
                rb.position = new Vector3(rb.position.x, rb.position.y - amount, rb.position.z);
                playerIsCrouching = true;
            } else {
                EnforceExactCharacterHeight();
            }
        }

        private void UnCrouch() {
            if (cc.height < playerFullHeight - playerCrouchedHeightTolerance) {
                var sphereCastRadius = cc.radius * headCheckRadiusMultiplier;
                var headroomBufferDistance = 0.05f;
                var sphereCastTravelDistance = (cc.bounds.extents.y + headroomBufferDistance) - sphereCastRadius;
                if (!(Physics.SphereCast(playerCenterPoint,
                                         sphereCastRadius,
                                         rb.transform.up,
                                         out _,
                                         sphereCastTravelDistance))) {
                    var time = Time.fixedDeltaTime * crouchTimeMultiplier;
                    var amount = Mathf.Lerp(0f, crouchAmount, time);
                    cc.height += amount;
                    cc.center = new Vector3(cc.center.x, cc.center.y - (amount * 0.5f), cc.center.z);
                    rb.position = new Vector3(rb.position.x, rb.position.y + amount, rb.position.z);
                } else {
                    playerIsCrouching = false;
                    EnforceExactCharacterHeight();
                }
            }
        }

        private void EnforceExactCharacterHeight() {
            if (playerIsCrouching) {
                cc.height = playerCrouchedHeight;
                cc.center = new Vector3(0, crouchAmount * 0.5f, 0f);
            } else {
                cc.height = playerFullHeight;
                cc.center = Vector3.zero;
            }
        }

        private Vector3 PlayerRun() {
            var calculatedPlayerRunSpeed = playerMoveInput;
            if (input.run && input.moveIsPressed && !playerIsCrouching) {
                calculatedPlayerRunSpeed *= runMultiplier;
            }
            return calculatedPlayerRunSpeed;
        }

        private void PlayerInfoCapture() {
            if (playerIsGrounded && groundCheckHit.collider) {
                lastGroundCheckHit = groundCheckHit;
                playerMoveInputAtLastGroundCheckHit = playerMoveInput;
            }
        }

        private float PlayerFallGravity() {
            var gravity = playerMoveInput.y;
            if (playerIsGrounded || playerIsAscendingStairs || playerIsDescendingStairs) {
                gravityFallCurrent = gravityFallMin;
            } else {
                playerFallTimer -= Time.deltaTime;
                if (playerFallTimer < 0f) {
                    var gravityFallMax = movementMultiplier * runMultiplier * 5f;
                    var gravityFallIncrementAmount = (gravityFallMax - gravityFallMin) * 0.1f;
                    if (gravityFallCurrent < gravityFallMax) {
                        gravityFallCurrent += gravityFallIncrementAmount;
                    }
                    playerFallTimer = gravityFallIncrementTime;
                }
                gravity = -gravityFallCurrent;
            }
            return gravity;
        }

        private float PlayerJump() {
            var calculatedJumpInput = playerMoveInput.y;
            SetJumpTimeCounter();
            SetCoyoteTimeCounter();
            SetJumpPressedBufferCounter();
            if (jumpBufferTimeCounter > 0f && !isJumping && coyoteTimeCounter > 0f) {
                // Prevent jumping up too steep slopes
                if (Vector3.Angle(rb.transform.up, groundCheckHit.normal) < maxSlopeAngle) {
                    KickStuffOutFromUnder();
                    calculatedJumpInput = initialJumpForceMultiplier;
                    isJumping = true;
                    jumpBufferTimeCounter = 0f;
                    coyoteTimeCounter = 0f;
                }
            } else if (input.jump && isJumping && !playerIsGrounded && jumpTimeCounter > 0f) {
                calculatedJumpInput = initialJumpForceMultiplier * continualJumpForceMultiplier;
            } else if (isJumping && playerIsGrounded) {
                isJumping = false;
            }
            return calculatedJumpInput;
        }

        private void KickStuffOutFromUnder() {
            // todo maybe a kick force as attack?
            if (lastGroundCheckHit.collider.attachedRigidbody) {
                var force = playerMoveInputAtLastGroundCheckHit
                          * jumpReactionForceMultiplier
                          * lastGroundCheckHit.collider.attachedRigidbody.mass;
                var forceDirection = rb.transform.TransformDirection(force);
                groundCheckHit.collider.attachedRigidbody.AddForceAtPosition(-forceDirection,
                                                                             lastGroundCheckHit.point,
                                                                             ForceMode.Impulse);
            }
        }

        private void SetJumpTimeCounter() {
            if (isJumping && !playerIsGrounded) {
                jumpTimeCounter -= Time.fixedDeltaTime;
            } else {
                jumpTimeCounter = jumpTime;
            }
        }

        private void SetCoyoteTimeCounter() {
            if (playerIsGrounded) {
                coyoteTimeCounter = coyoteTime;
            } else {
                coyoteTimeCounter -= Time.fixedDeltaTime;
            }
        }

        private void SetJumpPressedBufferCounter() {
            if (!jumpPressedLastFrame && input.jump) {
                jumpBufferTimeCounter = jumpBufferTime;
            } else if (jumpBufferTimeCounter > 0f) {
                jumpBufferTimeCounter -= Time.fixedDeltaTime;
            }
            jumpPressedLastFrame = input.jump;
        }
    }
}