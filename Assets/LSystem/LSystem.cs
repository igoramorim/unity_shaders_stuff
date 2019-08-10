using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LSystem : MonoBehaviour {

    [Range(1, 100)]
    public int step;
    [Range(1, 359)]
    public int angle;
    public string ruleA;
    public string ruleB;
    public string forwardCode;
    public string leftCode;
    public string rightCode;
    public string ruleACode;
    public string ruleBCode;

    private LineRenderer line;

    void Start() {
        line = GetComponent<LineRenderer>();
        // initial position
        // line.positionCount = 1;
        // line.SetPosition(0, new Vector3(0, 0, 0));

        // addPosition();

        // A(3);

        /*foreach(char w in "igor") {
            Debug.Log(w);
            if ("g".Equals(w.ToString())) {
                Debug.Log("ok");
            }
        }*/
    }

    void Update() {
        // Debug.Log(line.positionCount);

        // transform.position += transform.forward * step;
    }

    private void A(int n) {
        if (n > 0) {
            L(ruleA, n);
        }
    }

    private void B(int n) {
        if (n > 0) {
            L(ruleB, n);
        }
    }

    // TODO: loop in a string
    private void L(string axiom, int n) {
        // Debug.Log(axiom + " || " + n.ToString());
        foreach(char l in axiom) {
            Debug.Log("letter: " + l);
            if (leftCode.Equals(l.ToString())) {
                Debug.Log("virar esquerda");
            } else if (rightCode.Equals(l.ToString())) {
                Debug.Log("virar direita");
            } else if (ruleACode.Equals(l.ToString())) {
                Debug.Log("A(n-1)");
            } else if (ruleBCode.Equals(l.ToString())) {
                Debug.Log("B(n-1)");
            } else if (forwardCode.Equals(l.ToString())) {
                Debug.Log("frente");
            }
        }
    }

    private void moveForward() {

    }

    private void addPosition() {
        Vector3 pos = line.GetPosition(line.positionCount - 1);
        Debug.Log("posicao atual: " + pos);

        line.positionCount++;
        Debug.Log("tamanho: " + line.positionCount);

        line.SetPosition(line.positionCount - 1, pos);
    }

    private void turnLeft() {

    }

    private void turnRight() {

    }

}
