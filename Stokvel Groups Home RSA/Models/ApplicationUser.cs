﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stokvel_Groups_Home_RSA.Models;

public class ApplicationUser : IdentityUser
{

    [Required(ErrorMessage = "Please Fill In Your Name")]
    [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
    [Column("FirstName")]
    [Display(Name = "First Name")]
    public string? FirstName { get; set; }


    [Required(ErrorMessage = "Please Fill In Your Lastname")]
    [Display(Name = "Last Name")]
    [StringLength(50)]
    public string? LastName { get; set; }

    [NotMapped]
    [DisplayName("Profile Picture")]
    [DataType(DataType.Upload)]
    [FileExtensions(Extensions = "jpg,png,gif,jpeg,bmp,svg")]
    public IFormFile? ProfilePicture { get; set; }
    public string? MemberPhotoPath { get; set; }
    public string? MemberFileName { get; set; }


    [Required(ErrorMessage = "Please Fill In Your address")]
    public string? Address { get; set; }
    [Required(ErrorMessage = "Please Fill In Your City")]
    public string? City { get; set; }


    [Required(ErrorMessage = "Please Fill In Your Province")]
    public string? Province { get; set; }
    [Required(ErrorMessage = "Please Fill In Your Zip")]
    public int Zip { get; set; }


    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime Date { get; set; }


    public virtual IEnumerable<Account?>? UserAccounts { get; set; }
    public virtual AccountProfile? AccountProfiles { get; set; }
    public virtual ICollection<Wallet?>? Wallets { get; set; }



}
