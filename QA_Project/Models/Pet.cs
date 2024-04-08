using System.ComponentModel.DataAnnotations;


namespace QA_Project.Models
{
    public class Pet
    {
        [Key]
        public int PetId { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu")]
        [StringLength(50, ErrorMessage = "Numele nu poate avea mai mult de 50 de caractere")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Specia este obligatorie")]
        [StringLength(100, ErrorMessage = "Specia nu poate avea mai mult de 100 de caractere")]
        public string Species { get; set; }

        [Required(ErrorMessage = "Rasa este obligatorie")]
        [StringLength(100, ErrorMessage = "Rasa nu poate avea mai mult de 100 de caractere")]
        public string Breed { get; set; }

        [Required(ErrorMessage = "Varsta este obligatorie")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Marimea este obligatorie")]
        public int Size { get; set; }

        [Required(ErrorMessage = "Genul este obligatoriu")]
        public bool Sex { get; set; }

        [Required(ErrorMessage = "Culoarea este obligatorie")]
        [StringLength(100, ErrorMessage = "Culoarea nu poate avea mai mult de 100 de caractere")]
        public string Color { get; set; }
        public bool Vaccined { get; set; }
        public bool Sterilized { get; set; }

        [Required(ErrorMessage = "Locatia este obligatorie")]
        [StringLength(100, ErrorMessage = "Locatia nu poate avea mai mult de 100 de caractere")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Descrierea este obligatorie")]
        [StringLength(700, ErrorMessage = "Descrierea nu poate avea mai mult de 700 de caractere")]
        [MinLength(15, ErrorMessage = "Descrierea trebuie sa aiba mai mult de 15 caractere")]
        public string Description { get; set; }
        public string? Image { get; set; }
    }
}
