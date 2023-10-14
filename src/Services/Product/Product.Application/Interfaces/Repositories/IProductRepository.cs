﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<bool> IsBrandUsed(int brandId);
}