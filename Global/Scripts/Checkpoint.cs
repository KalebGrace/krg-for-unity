using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    //[ExecuteInEditMode]
    public class Checkpoint : MonoBehaviour
    {
        public LetterName checkpointName;

        public bool autoNameGameObject = true;

        public bool saveOnTriggerEnter = true;

        private void OnValidate()
        {
            if (autoNameGameObject)
            {
                gameObject.name = "Checkpoint" + checkpointName;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (saveOnTriggerEnter && other.tag == PlayerCharacter.TAG && !G.save.IsCurrentCheckpoint(checkpointName))
            {
                G.save.SaveCheckpoint(checkpointName);
            }
        }

        /*
        private LetterName previousCheckpointName;

        private static readonly Dictionary<LetterName, Checkpoint> dict = new Dictionary<LetterName, Checkpoint>();

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
            for (int i = 1; i <= (int)LetterName.Last; ++i)
            {
                checkpointName = (LetterName)i;

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
