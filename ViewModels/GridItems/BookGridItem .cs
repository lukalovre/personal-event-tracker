using System;

public record BookGridItem(
    int ID,
    int Index,
    string Title,
    string Author,
    int Year,
    int? Rating,
    int Pages,
    DateTime? LastDate
);
