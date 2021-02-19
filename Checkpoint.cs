using UnityEngine;

namespace _0G.Legacy
{
    //[ExecuteInEditMode]
    public class Checkpoint : MonoBehaviour
    {
        public AlphaBravo checkpointName;

        public bool autoNameGameObject = true;

        public bool saveOnTriggerEnter = true;

        [Enum(typeof(FacingDirection))]
        public int spawnFacingDirection;

        private void OnValidate()
        {
            if (autoNameGameObject)
            {
                gameObject.name = "Checkpoint" + checkpointName;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (saveOnTriggerEnter && G.obj.IsPlayerCharacter(other) && !G.save.IsCurrentCheckpoint(checkpointName))
            {
                G.save.SaveCheckpoint(checkpointName);
            }
        }

        /*
        private AlphaBravo previousCheckpointName;

        private static readonly Dictionary<AlphaBravo, Checkpoint> dict = new Dictionary<AlphaBravo, Checkpoint>();

        private void Awake()
        {
            dict.Add(checkpointName, this);

            previousCheckpointName = checkpointName;
        }

        private void OnDestroy()
        {
            dict.Remove(checkpointName);
        }

        public void OnValidate()
        {
            if (checkpointName == 0 || checkpointName != previousCheckpointName)
            {
                OnNameChange();
            }

            gameObject.name = "Checkpoint_" + checkpointName.ToString()[0];
        }

        private void OnNameChange()
        {
            dict.Remove(previousCheckpointName);

            if (checkpointName == 0)
            {
                AutoIncrement();

                dict.Add(checkpointName, this);
            }
            else if (dict.ContainsKey(checkpointName))
            {
                //swap

                var other = dict[checkpointName];

                dict.Remove(checkpointName);

                dict.Add(checkpointName, this);

                if (previousCheckpointName == 0)
                {
                    other.AutoIncrement();
                }
                else
                {
                    other.checkpointName = previousCheckpointName;
                }

                dict.Add(other.checkpointName, other);
            }
            else
            {
                dict.Add(checkpointName, this);
            }

            previousCheckpointName = checkpointName;
        }

        public void AutoIncrement()
        {
            for (int i = 1; i <= (int)AlphaBravo.Last; ++i)
            {
                checkpointName = (AlphaBravo)i;

                if (!dict.ContainsKey(checkpointName))
                {
                    return;
                }
            }

            G.U.Err("Unable to AutoIncrement this checkpoint.", this);
        }

        */
    }
}