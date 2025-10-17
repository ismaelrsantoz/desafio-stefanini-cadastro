import React, { useState } from 'react';
import { TextField, Button, Box, Typography, Paper } from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';

function PessoaFilter({ onSearch }) {
  const [filters, setFilters] = useState({
    nome: '',
    cpf: ''
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFilters(prevFilters => ({ ...prevFilters, [name]: value }));
  };

  const handleSearch = (e) => {
    e.preventDefault();
    onSearch(filters);
  };

  return (
    <Paper elevation={3} sx={{ p: 3, mb: 4 }}>
      <Typography variant="h6" component="h2" gutterBottom>
        Consultar Pessoas
      </Typography>

      {/* --- INÍCIO DA ADIÇÃO --- */}
      <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
        Para ver todos os registos, deixe os campos em branco e clique em "Buscar".
      </Typography>
      {/* --- FIM DA ADIÇÃO --- */}
      
      <Box 
        component="form" 
        onSubmit={handleSearch}
        sx={{ display: 'flex', alignItems: 'center', gap: 2 }}
      >
        <TextField 
          label="Buscar por Nome" 
          name="nome" 
          value={filters.nome}
          onChange={handleChange}
          variant="outlined" 
          fullWidth 
        />
        <TextField 
          label="Buscar por CPF" 
          name="cpf" 
          value={filters.cpf}
          onChange={handleChange}
          variant="outlined" 
          fullWidth 
        />
        <Button 
          type="submit" 
          variant="contained" 
          startIcon={<SearchIcon />}
          sx={{ height: '56px', px: 4 }}
        >
          Buscar
        </Button>
      </Box>
    </Paper>
  );
}

export default PessoaFilter;