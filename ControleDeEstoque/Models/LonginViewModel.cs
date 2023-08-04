using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ControleDeEstoque.Models
{
    public class LonginViewModel
    {
        [Required(ErrorMessage = "Informe o usuário")] // validação
        [Display(Name = "Usuário")] // passando para fronte 
        public string Usuario { get; set; }
       
        [Required(ErrorMessage ="Informe a senha")]
        [DataType(DataType.Password)] // tipo password
        public string Senha { get; set; }
      
        [Required]
        [Display(Name = "Lembra Me")]
        public bool LembraMe { get; set; }



    }


}