using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Company
/// </summary>

[PetaPoco.PrimaryKey("id")]
public class Company
{
	public Company()
	{
		//
		// TODO: Add constructor logic here
		//
	}   
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public string Note { get; set; }
    //public bool Active { get; set; }

}