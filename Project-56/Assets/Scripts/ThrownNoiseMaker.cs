using UnityEngine;

public class ThrownNoiseMaker : MonoBehaviour
{
    [SerializeField] GameObject stimulusPrefab;
    [SerializeField] float startingAwareness = 3f;
    [SerializeField] Stimulus stimulus;
    [SerializeField] AudioClip impact;

    private void OnCollisionEnter(Collision collision) {
        if (stimulus != null) {
            stimulus.transform.position = transform.position;
            stimulus.awareness = startingAwareness;
        }
        else {
            var stimObj = Instantiate(stimulusPrefab, transform.position, Quaternion.identity) as GameObject;
            if (stimObj.TryGetComponent<Stimulus>(out Stimulus _stimulus)) {
                stimulus = _stimulus;
                stimulus.UpdateStim(startingAwareness);
            }
        }

        AudioSource.PlayClipAtPoint(impact, transform.position);
    }
}
