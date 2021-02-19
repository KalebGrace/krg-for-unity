using UnityEngine;

namespace _0G.Legacy
{
    public class Movement
    {
        public const float defaultStepHeight = 0.5f;

        delegate void MovementHandler(Vector3 v3);

        bool _doesRespectRigidbodyFreeze; //TODO: This needs to be used in more places.
        SpatialOptions _spatialOptions;

        float safetyCushion
        {
            get
            {
                //NOTE: Don't try and cache Physics.defaultContactOffset early, as it might come out as 0.
                return 3f * Physics.defaultContactOffset;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Movement"/> class.
        /// doesRespectRigidbodyFreeze will be false.
        /// </summary>
        /// <param name="options">Options. Old default was SpatialOptions.AdditiveCardinal.</param>
        public Movement(SpatialOptions options)
        {
            _spatialOptions = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Movement"/> class.
        /// doesRespectRigidbodyFreeze will be set as specified.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="doesRespectRigidbodyFreeze">If set to <c>true</c> does respect rigidbody freeze.</param>
        public Movement(SpatialOptions options, bool doesRespectRigidbodyFreeze)
        {
            _spatialOptions = options;
            _doesRespectRigidbodyFreeze = doesRespectRigidbodyFreeze;
        }

        /// <summary>
        /// Cause the specified transform to fall (if possible) using the specified rigidbody and other params.
        /// </summary>
        /// <param name="tf">The transform.</param>
        /// <param name="body">The rigidbody.</param>
        /// <param name="dist">The distance.</param>
        /// <param name="stepHeight">The step height. If you're not sure, use Movement.DEFAULT_STEP_HEIGHT.</param>
        /// <param name="passthroughLayers">Passthrough layers (optional).</param>
        public void Fall(
            Transform tf,
            Rigidbody body,
            float dist,
            float stepHeight,
            params int[] passthroughLayers
        )
        {
            //Ensure the user-supplied stepHeight adheres to the restrictions.
            float adjustedStepHeight = GetAdjustedStepHeight(stepHeight);

            //In Unity 5, rigidbodies flush with the ground plane
            //no longer hit it with SweepTestAll. This means we
            //need to lift before falling, as we do in MoveBasic(...).
            //Also, safetyCushion doesn't always seem to be enough,
            //so we're using adjustedStepHeight instead.
            //TODO: This all may be because I wasn't calling this function in a FixedUpdate for SoAm.
            //I should try this without a lift in a FixedUpdate loop to see if it still works.
            //TODO: If that doesn't fix anything, I should also attempt to simply check
            //other.contacts[0].normal.y in OnCollisionEnter, like I do in OSH.
            Vector3 lift = new Position(Direction.Above, adjustedStepHeight);
            tf.position += lift;

            TranslateOutcomes(new Position(Direction.Below, dist + adjustedStepHeight),
                v3 =>
                { //Success.
                    Debug.DrawRay(tf.position, v3, Color.cyan, 3f);
                    tf.position += v3;
                },
                v3 =>
                { //Obstruction.
                    Debug.DrawRay(tf.position, v3, Color.yellow, 1f);
                },
                v3 =>
                { //Partial.
                    Debug.DrawRay(tf.position, v3, Color.cyan, 3f);
                    tf.position += v3;
                },
                null,
                body, passthroughLayers);
        }

        /// <summary>
        /// Move the specified transform (if possible) using the specified rigidbody and other params.
        /// </summary>
        /// <param name="tf">The transform.</param>
        /// <param name="body">The rigidbody.</param>
        /// <param name="dir">The direction.</param>
        /// <param name="dist">The distance.</param>
        /// <param name="stepHeight">The step height. If you're not sure, use Movement.DEFAULT_STEP_HEIGHT.</param>
        /// <param name="passthroughLayers">Passthrough layers (optional).</param>
        public void MoveBasic(
            Transform tf,
            Rigidbody body,
            Direction dir,
            float dist,
            float stepHeight,
            params int[] passthroughLayers
        )
        {
            switch (dir.GetDirectionType())
            {
                case DirectionType.Cardinal:
                case DirectionType.Ordinal:
                    break;
                default:
                    //TODO: Is this restriction still relevant? If so, why? SoAm only?
                    //May have had to do with "Remainder" of AttemptMove, but this has been updated.
                    G.U.Err("The direction parameter must be of " +
                        "an absolute direction type. " +
                        "Its current value is \"{0}\" " +
                        "and its direction type is \"{1}\".",
                        dir,
                        dir.GetDirectionType());
                    return;
            }
            Position posDelta = new Position(dir, dist, _spatialOptions);
            MoveBasic(tf, body, posDelta, stepHeight, passthroughLayers);
        }

        /// <summary>
        /// Move the specified transform (if possible) using the specified rigidbody and other params.
        /// </summary>
        /// <param name="tf">The transform.</param>
        /// <param name="body">The rigidbody.</param>
        /// <param name="travelVector">The travel vector.</param>
        /// <param name="stepHeight">The step height. If you're not sure, use Movement.DEFAULT_STEP_HEIGHT.</param>
        /// <param name="passthroughLayers">Passthrough layers (optional).</param>
        public void MoveBasic(
            Transform tf,
            Rigidbody body,
            Vector3 travelVector,
            float stepHeight,
            params int[] passthroughLayers
        )
        {
            Position posDelta = new Position(travelVector, _spatialOptions);
            MoveBasic(tf, body, posDelta, stepHeight, passthroughLayers);
        }

        /// <summary>
        /// Move the specified transform (if possible) using the specified rigidbody and other params.
        /// </summary>
        /// <param name="tf">The transform.</param>
        /// <param name="body">The rigidbody.</param>
        /// <param name="posDelta">The position delta.</param>
        /// <param name="stepHeight">The step height. If you're not sure, use Movement.DEFAULT_STEP_HEIGHT.</param>
        /// <param name="passthroughLayers">Passthrough layers (optional).</param>
        public void MoveBasic(
            Transform tf,
            Rigidbody body,
            Position posDelta,
            float stepHeight,
            params int[] passthroughLayers
        )
        {
            //Ensure the user-supplied stepHeight adheres to the restrictions.
            float adjustedStepHeight = GetAdjustedStepHeight(stepHeight);

            //#1: Lift directly above current position. No need to check for collisions here.
            Vector3 lift = new Position(Direction.Above, adjustedStepHeight);
            Debug.DrawRay(tf.position, lift, Color.white, 1f);
            tf.position += lift;

            //#2: Attempt to move as intended.
            AttemptMove(posDelta, tf, body, passthroughLayers);

            //#3: Drop back down to the original height (or the ground, if closer).
            TranslateOutcomes(new Position(Direction.Below, adjustedStepHeight),
                v3 =>
                { //Success.
                    Debug.DrawRay(tf.position, v3, Color.white, 1f);
                    tf.position += v3;
                },
                null,
                v3 =>
                { //Partial.
                    Debug.DrawRay(tf.position, v3, Color.green, 3f);
                    tf.position += v3;
                },
                null,
                body, passthroughLayers);
        }

        void AttemptMove(
            Position attempt,
            Transform tf,
            Rigidbody body,
            params int[] passthroughLayers
        )
        {
            TranslateOutcomes(attempt,
                v3 =>
                { //Success.
                    Debug.DrawRay(tf.position, v3, Color.white, 1f);
                    tf.position += v3;
                },
                v3 =>
                { //Obstruction.
                    Debug.DrawRay(tf.position, v3 * 2f, Color.red, 2f);
                },
                v3 =>
                { //Partial.
                    Debug.DrawRay(tf.position, v3, Color.white, 1f);
                    tf.position += v3;
                },
                v3 =>
                { //Remainder. (Attempt to move in a different direction against an angled surface.)
                    bool tryRemainder = true;
                    float distRem = v3.magnitude;
                    Direction dir = attempt.initialDirection;
                    SpatialOptions options = SpatialOptions.None;
                    //TODO: Find out if these adjustments to options/distRem are correct.
                    //It should already be handling this stuff in Position.SetDirectionMagnitude(...).
                    //Also, take into account the value of _spatialOptions.
                    switch (dir.GetDirectionType())
                    {
                        case DirectionType.Cardinal:
                            options = SpatialOptions.AdditiveCardinal; //For remaining ordinal movement.
                            break;
                        case DirectionType.Ordinal:
                            distRem *= Mathf.Sqrt(0.5f); //For remaining cardinal movement.
                            break;
                        default:
                            tryRemainder = false;
                            break;
                    }
                    if (_doesRespectRigidbodyFreeze)
                    {
                        int freezeX = (int) body.constraints & (int) RigidbodyConstraints.FreezePositionX;
                        int freezeZ = (int) body.constraints & (int) RigidbodyConstraints.FreezePositionZ;
                        switch (dir)
                        {
                            case Direction.North:
                            case Direction.South:
                                tryRemainder &= freezeX <= 0;
                                break;
                            case Direction.East:
                            case Direction.West:
                                tryRemainder &= freezeZ <= 0;
                                break;
                            case Direction.NorthEast:
                            case Direction.SouthEast:
                            case Direction.SouthWest:
                            case Direction.NorthWest:
                                tryRemainder &= freezeX <= 0 && freezeZ <= 0;
                                break;
                            default:
                                //TODO: Find out if I need anything here.
                                break;
                        }
                    }
                    if (tryRemainder)
                    {
                        float cwDistAllowed = 0f;
                        float ccwDistAllowed = 0f;
                        Direction cwDir = dir.GetClockwiseDirection();
                        Direction ccwDir = dir.GetCounterClockwiseDirection();
                        Position cwPos = new Position(cwDir, distRem, options);
                        Position ccwPos = new Position(ccwDir, distRem, options);
                        ForecastTranslate(body, cwPos, out cwDistAllowed, passthroughLayers);
                        ForecastTranslate(body, ccwPos, out ccwDistAllowed, passthroughLayers);

                        Vector3 v3rem;
                        if (cwDistAllowed > ccwDistAllowed)
                        {
                            v3rem = cwPos.v3.normalized * cwDistAllowed;
                        }
                        else
                        {
                            v3rem = ccwPos.v3.normalized * ccwDistAllowed;
                        }
                        tf.position += v3rem;
                        Debug.DrawRay(tf.position, v3rem, Color.white, 1f);
                    }
                },
                body, passthroughLayers);
        }

        /// <summary>
        /// Wrapper function that handles all possible outcomes of a translation attempt.
        /// It will either use the "success" handler, or all three other handlers combined (as provided).
        /// </summary>
        /// <param name="attempt">The translation attempt (direction + magnitude).</param>
        /// <param name="success">What to do when translation attempt is completely allowed (v3 = original attempt).</param>
        /// <param name="obstruction">What to do when there is any kind of obstruction (v3 = original attempt).</param>
        /// <param name="partial">What to do with the partially allowed translation (v3 = distance allowed).</param>
        /// <param name="remainder">What to do with the remainder of the attempt (v3 = disallowed part).</param>
        /// <param name="body">The rigidbody.</param>
        /// <param name="passthroughLayers">Passthrough layers (optional).</param>
        void TranslateOutcomes(
            Position attempt,
            MovementHandler success,
            MovementHandler obstruction,
            MovementHandler partial,
            MovementHandler remainder,
            Rigidbody body,
            params int[] passthroughLayers
        )
        {
            float distAllowed;
            Vector3 attemptV3 = attempt.v3;
            Vector3 attemptV3_Normalized = attemptV3.normalized;

            if (ForecastTranslate(body, attempt, out distAllowed, passthroughLayers))
            {
                if (success != null)
                {
                    success(attemptV3);
                }
            }
            else
            {
                if (obstruction != null)
                {
                    obstruction(attemptV3);
                }
                if (partial != null && distAllowed > 0f)
                {
                    partial(attemptV3_Normalized * distAllowed);
                }
                if (remainder != null)
                {
                    remainder(attemptV3_Normalized * (attemptV3.magnitude - distAllowed));
                }
            }
        }

        /// <summary>
        /// Forecasts the acceptable translation of a rigidbody.
        /// </summary>
        /// <returns><c>true</c>, if translation attempt is completely allowed,
        /// <c>false</c> if it is disallowed or only partially allowed.</returns>
        /// <param name="body">The rigidbody.</param>
        /// <param name="attempt">The translation attempt (direction + magnitude).</param>
        /// <param name="distAllowed">The distance magnitude that is actually allowed.</param>
        /// <param name="passthroughLayers">Passthrough layers (optional).</param>
        bool ForecastTranslate(
            Rigidbody body,
            Position attempt,
            out float distAllowed,
            params int[] passthroughLayers
        )
        {
            Vector3 attemptV3 = attempt.v3;
            distAllowed = attemptV3.magnitude; //Initialization.

            //TODO: Check the following line to make sure it's not assuming AdditiveCardinal.
            float safetyMulti = attempt.initialDirection.GetDirectionType() == DirectionType.Ordinal ? Mathf.Sqrt(2f) : 1f;
            float adjustedSafetyCushion = safetyCushion * safetyMulti;

            //Prevent jittering when using MoveBasic under the following circumstances.
            if (!body.isKinematic && body.useGravity && attempt.initialDirection == Direction.Below)
            {
                adjustedSafetyCushion = 0f;
            }

            RaycastHit[] hits = body.SweepTestAll(attemptV3, distAllowed);

            RaycastHit hit;
            int hitCount = hits.Length;

            bool isPassthrough;
            int passthroughLayerCount = passthroughLayers.Length;

            for (int i = 0; i < hitCount; i++)
            {
                hit = hits[i];

                if (hit.collider.isTrigger)
                {
                    continue;
                }

                isPassthrough = false;
                for (int j = 0; j < passthroughLayerCount; j++)
                {
                    if (hit.transform.gameObject.layer == passthroughLayers[j])
                    {
                        isPassthrough = true;
                        break;
                    }
                }
                if (isPassthrough)
                {
                    continue;
                }

                distAllowed = Mathf.Min(distAllowed, hit.distance - adjustedSafetyCushion);
                if (distAllowed <= 0f)
                {
                    distAllowed = 0f;
                    break;
                }
            }

            return Mathf.Approximately(distAllowed, attemptV3.magnitude);
        }

        float GetAdjustedStepHeight(float stepHeight)
        {
            return Mathf.Max(stepHeight + safetyCushion, safetyCushion);
        }
    }
}