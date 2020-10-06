using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace librarySP.Database.enums
{
    public enum SearchBooks
    { 
        [Display(Name ="Название")]
        BookName,
        [Display(Name = "Описание")]
        BookDescription,
        [Display(Name = "Жанр")]
        BookGenre,
        [Display(Name = "Год выпуска")]
        BookYear,
        [Display(Name = "Автор")]
        BookAuthor,
        [Display(Name = "Издательство")]
        BookPublisher
    }
}
