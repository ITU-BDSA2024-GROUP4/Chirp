﻿namespace Chirp.Core;

public interface IAuthorRepository
{
    public List<Author> GetAuthor(string email);
    public Author AddAuthor(string name, string email);
    public List<Author> GetAuthorUserName(string userName);

    
}