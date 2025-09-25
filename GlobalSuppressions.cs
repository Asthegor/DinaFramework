// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage( "CodeQuality", "IDE0079:Retirer la suppression inutile",
    Justification = "La suppression était volontaire pour ignorer un warning dans le code généré / pattern spécifique à Key<T>. FromString, qui est sûr et ne nécessite pas de modification.",
    Scope = "member", Target = "~M:DinaFramework.Services.Key`1.FromString(System.String)~DinaFramework.Services.Key{`0}")]

[assembly: SuppressMessage( "CodeQuality", "IDE0079:Retirer la suppression inutile",
    Justification = "La suppression était volontaire pour ignorer un warning IDE sur des patterns internes à Text, qui sont intentionnels et ne posent pas de problème de qualité ou de maintenance.",
    Scope = "type", Target = "~T:DinaFramework.Graphics.Text")]

[assembly: SuppressMessage("Design", "CA1032:Implémenter des constructeurs d'exception standard", 
    Justification = "Nouvelle forme d'écriture C# 12",
    Scope = "type", Target = "~T:DinaFramework.Exceptions.InvalidSceneTypeException")]

[assembly: SuppressMessage("Design", "CA1032:Implémenter des constructeurs d'exception standard",
    Justification = "Nouvelle forme d'écriture C# 12",
    Scope = "type", Target = "~T:DinaFramework.Exceptions.DuplicateDictionaryKeyException")]

[assembly: SuppressMessage("Design", "CA1032:Implémenter des constructeurs d'exception standard",
    Justification = "Nouvelle forme d'écriture C# 12",
    Scope = "type", Target = "~T:DinaFramework.Exceptions.SpriteBatchNotBeginException")]

[assembly: SuppressMessage("CodeQuality", "IDE0079:Retirer la suppression inutile",
    Justification = "La suppression est volontaire : certaines suppressions de warnings dans le code sont nécessaires pour le fonctionnement ou le pattern utilisé et ne doivent pas être retirées.")]

