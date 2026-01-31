using UnityEngine;

/// <summary>
/// Абстракция ввода для движения персонажа.
/// Единственная ответственность: предоставлять данные ввода.
/// </summary>
public interface IMovementInput
{
    /// <summary>
    /// Направление движения в плоскости XZ (горизонталь). Нормализовано или с учётом величины нажатия.
    /// </summary>
    Vector3 MoveDirection { get; }

    /// <summary>
    /// Было ли в текущем кадре нажато действие "Прыжок".
    /// </summary>
    bool JumpPressedThisFrame { get; }
}
