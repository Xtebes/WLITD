using UnityEngine;
public class test : MonoBehaviour
{   
    float Distancia(float Ax, float Ay, float Bx, float By)
    {
        return Mathf.Sqrt(Mathf.Pow((Bx - Ax),2) + Mathf.Pow((By - Ay),2));
    }
    string Horas(int segundos)
    {
        int horas = segundos / 3600;
        int minutos = (segundos - (3600 * horas)) / 60;
        segundos -= (3600 * horas) + (minutos * 60);
        return $"{horas}:{minutos}:{segundos}";
    }
    bool Palíndromo(string A)
    {
        for(int i = 0; i < A.Length/2; i++)
        {
            if (char.ToLower(A[i]) != char.ToLower(A[A.Length - 1 - i]))
            {
                return false;
            }
        }
        return true;
    }
    void ChangeSpriteRendererColor()
    {
        float randomR = Random.Range(0f, 1f);
        float randomG = Random.Range(0f, 1f);
        float randomB = Random.Range(0f, 1f);
    }
}
public class Misterio : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InvokeRepeating("ChangeSpriteRendererColor", 0, 2);
    }
    void ChangeSpriteRendererColor()
    {
        float randomR = Random.Range(0f, 1f);
        float randomG = Random.Range(0f, 1f);
        float randomB = Random.Range(0f, 1f);
        spriteRenderer.color = new Color(randomR, randomG, randomB);
    }
}
