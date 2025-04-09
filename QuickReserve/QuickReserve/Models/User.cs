using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

public class User
{
    public string UserId { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    //public string Password { get; set; }
    public string Role { get; set; }
    public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("o");
    public string ProfileImage { get; set; }
    public List<string> LikedFoods { get; set; }
    [JsonIgnore]
    public ImageSource ProfileImageSource { get; set; }
}