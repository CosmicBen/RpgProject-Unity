using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Core
{
    [Serializable]
    public class Condition
    {
        [Serializable]
        class Predicate
        {
            [SerializeField] private string predicate;
            [SerializeField] private string[] parameters;
            [SerializeField] private bool negate = false;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (IPredicateEvaluator evaluator in evaluators)
                {
                    bool? result = evaluator.Evaluate(predicate, parameters);
                    if (result == null) { continue; }
                    if (result == negate) { return false; }
                }

                return true;
            }
        }

        [Serializable]
        class Disjunction
        {
            [SerializeField]
            Predicate[] or;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (Predicate predicate in or)
                {
                    if (predicate.Check(evaluators))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [SerializeField] Disjunction[] and;

        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            foreach (Disjunction disjunction in and)
            {
                if (!disjunction.Check(evaluators))
                {
                    return false;
                }
            }

            return true;
        }
    }
}