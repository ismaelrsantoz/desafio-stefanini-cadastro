import React from 'react';
import { Paper, Typography, Grid, Box } from '@mui/material';
import PersonAddAlt1Icon from '@mui/icons-material/PersonAddAlt1';
import ManageSearchIcon from '@mui/icons-material/ManageSearch';

function Dashboard({ onNavigate }) {
  // Estilo base para os cards
  const cardStyle = {
    p: 4,
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    justifyContent: 'center',
    cursor: 'pointer',
    transition: 'transform 0.3s ease-in-out, box-shadow 0.3s ease-in-out, background-color 0.3s ease-in-out, color 0.3s ease-in-out',
    minHeight: '220px',
    borderRadius: '16px',
    backgroundColor: 'white', // Cor inicial para ambos os cards
    color: 'text.primary', // Cor do texto inicial
    '&:hover': {
      transform: 'translateY(-8px)',
      boxShadow: '0 12px 20px -10px rgba(0, 0, 0, 0.2)',
      backgroundColor: 'primary.main', // Muda para azul no hover
      color: 'white', // Muda o texto para branco no hover
      '& .MuiSvgIcon-root': { // Muda a cor do ícone no hover
        color: 'white',
      },
      '& .MuiTypography-colorTextSecondary': { // Muda a cor do texto secundário no hover
        color: 'rgba(255, 255, 255, 0.8)',
      }
    },
  };

  return (
    <Box>
      <Typography variant="h4" component="h2" gutterBottom align="center" sx={{ mb: 1, fontWeight: 'bold' }}>
        Bem-vindo!
      </Typography>
      <Typography variant="h6" color="text.secondary" align="center" sx={{ mb: 5 }}>
        O que gostaria de fazer hoje?
      </Typography>

      <Grid container spacing={5} justifyContent="center">
        {/* Card de Cadastro */}
        <Grid item xs={12} md={5}>
          <Paper 
            elevation={4} 
            sx={cardStyle}
            onClick={() => onNavigate('cadastro')}
          >
            <PersonAddAlt1Icon sx={{ fontSize: 60, mb: 2, color: 'primary.main', transition: 'color 0.3s ease-in-out' }} />
            <Typography variant="h5" component="h3" sx={{ fontWeight: 'bold' }}>
              Cadastrar Nova Pessoa
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mt: 1, transition: 'color 0.3s ease-in-out' }}>
              Adicionar um novo registo ao sistema.
            </Typography>
          </Paper>
        </Grid>

        {/* Card de Consulta */}
        <Grid item xs={12} md={5}>
          <Paper 
            elevation={4} 
            sx={cardStyle} 
            onClick={() => onNavigate('consulta')}
          >
            <ManageSearchIcon sx={{ fontSize: 60, mb: 2, color: 'action', transition: 'color 0.3s ease-in-out' }} />
            <Typography variant="h5" component="h3" sx={{ fontWeight: 'bold' }}>
              Consultar Pessoas
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mt: 1, transition: 'color 0.3s ease-in-out' }}>
              Buscar, editar ou remover registos existentes.
            </Typography>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
}

export default Dashboard;