//UNED
//Mitica X
//Jorge Arias Melendez
//Septiembre 2025
//Top 10 de jugadores ordenado manualmente (batallas desc, cristales desc, nombre asc)

using System;
using Miticax.Entidades;
using Miticax.Datos;

namespace Miticax.Logica
{
    public class RankingService
    {
        private readonly JugadorDatos _jugadorDatos;

        public RankingService(JugadorDatos jugadorDatos)
        {
            _jugadorDatos = jugadorDatos;
        }

        // Devuelve un arreglo con hasta 10 jugadores ordenados por:
        // 1) BatallasGanadas desc
        // 2) Cristales desc
        // 3) Nombre asc (ordinal)
        public JugadorEntidad[] Top10()
        {
            // Tomar snapshot (arreglo con tamaño = count)
            var snap = _jugadorDatos.GetAllSnapshot();

            // Hacer una copia (para no alterar el snapshot original) de igual tamanio
            var copia = new JugadorEntidad[snap.Length];
            for (int i = 0; i < snap.Length; i++)
            {
                copia[i] = snap[i];
            }

            // Ordenamiento: seleccion (selection sort) con nuestro comparador
            for (int i = 0; i < copia.Length - 1; i++)
            {
                int idxMejor = i;
                for (int j = i + 1; j < copia.Length; j++)
                {
                    if (EsMejor(copia[j], copia[idxMejor]))
                    {
                        idxMejor = j;
                    }
                }
                if (idxMejor != i)
                {
                    var tmp = copia[i];
                    copia[i] = copia[idxMejor];
                    copia[idxMejor] = tmp;
                }
            }

            // Cortar a top 10 si hay mas
            int n = copia.Length < 10 ? copia.Length : 10;
            var top = new JugadorEntidad[n];
            for (int k = 0; k < n; k++) top[k] = copia[k];
            return top;
        }

        // true si a es "mejor" que b segun criterios
        private bool EsMejor(JugadorEntidad? a, JugadorEntidad? b)
        {
            // Si ambos son nulos: no hay preferencia
            if (a == null && b == null) return false;

            // Si solo a existe, a es mejor
            if (a != null && b == null) return true;

            // Si solo b existe, a NO es mejor
            if (a == null && b != null) return false;

            // AQUI a y b no son nulos (para el analizador también)
            JugadorEntidad aj = a!;
            JugadorEntidad bj = b!;

            // 1) BatallasGanadas desc
            if (aj.BatallasGanadas > bj.BatallasGanadas) return true;
            if (aj.BatallasGanadas < bj.BatallasGanadas) return false;

            // 2) Cristales desc
            if (aj.Cristales > bj.Cristales) return true;
            if (aj.Cristales < bj.Cristales) return false;

            // 3) Nombre asc (usa cadenas seguras)
            string na = aj.Nombre ?? "";
            string nb = bj.Nombre ?? "";
            int cmp = string.CompareOrdinal(na, nb);
            return cmp < 0;
        }
    }
}
