import React, { useState } from 'react';
import {
  TextField,
  Button,
  Box,
  Paper,
  Typography,
  Avatar,
  CssBaseline
} from '@mui/material';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import { toast } from 'react-toastify';


const apiUrl = process.env.REACT_APP_API_URL || "http://localhost:5182";

function Copyright(props) {
  return (
    <Typography variant="body2" color="text.secondary" align="center" {...props}>
      {'Desenvolvido por Ismael Santos © '}
      {new Date().getFullYear()}
      {'.'}
    </Typography>
  );
}

function Login({ onLoginSuccess }) {
  const [username, setUsername] = useState('admin');
  const [password, setPassword] = useState('password123');

  const handleLogin = (e) => {
    e.preventDefault();
    // Usei a variável apiUrl para montar a URL completa da requisição.
    fetch(`${apiUrl}/api/Auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password }),
    })
    .then(response => {
      if (response.ok) return response.json();
      throw new Error('Credenciais inválidas');
    })
    .then(data => {
      toast.success('Login bem-sucedido!');
      onLoginSuccess(data.token);
    })
    .catch(error => {
      toast.error('Erro no login. Verifique suas credenciais.');
    });
  };

  return (
    <>
      <CssBaseline />
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          minHeight: '100vh',
          backgroundColor: '#f5f5f5',
          p: 4,
        }}
      >
        <Box
          sx={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            gap: { xs: 2, md: 8 },
            width: '100%',
            maxWidth: '1200px'
          }}
        >
          {/* Logo Esquerdo */}
          <Box
            component="img"
            src="/logo-stefanini.svg"
            alt="Logotipo Stefanini"
            sx={{
              display: { xs: 'none', md: 'block' },
              width: 250,
              height: 'auto',
              opacity: 0.6
            }}
          />

          {/* Card do Formulário de Login */}
          <Paper
            elevation={8}
            sx={{
              p: { xs: 3, sm: 4 },
              display: 'flex',
              flexDirection: 'column',
              alignItems: 'center',
              maxWidth: 420,
              width: '100%'
            }}
          >
            <Avatar sx={{ m: 1, bgcolor: 'primary.main' }}>
              <LockOutlinedIcon />
            </Avatar>
            <Typography component="h1" variant="h5">
              Login
            </Typography>
            <Box component="form" noValidate onSubmit={handleLogin} sx={{ mt: 1, width: '100%' }}>
              <TextField
                margin="normal"
                required
                fullWidth
                id="username"
                label="Usuário"
                autoFocus
                value={username}
                onChange={(e) => setUsername(e.target.value)}
              />
              <TextField
                margin="normal"
                required
                fullWidth
                name="password"
                label="Senha"
                type="password"
                id="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
              <Button
                type="submit"
                fullWidth
                variant="contained"
                sx={{ mt: 3, mb: 2 }}
              >
                Entrar
              </Button>
            </Box>
          </Paper>

          {/* Logo Direito */}
          <Box
            component="img"
            src="/logo-stefanini.svg"
            alt="Logotipo Stefanini"
            sx={{
              display: { xs: 'none', md: 'block' },
              width: 250,
              height: 'auto',
              opacity: 0.6
            }}
          />
        </Box>
        <Copyright sx={{ mt: 8, mb: 4 }} />
      </Box>
    </>
  );
}

export default Login;
