git pull origin master
 docker build -t kino2001/biylineapi .
docker stop  biylineapi
 docker remove biylineapi
docker run -d -p 2001:80 --name biylineapi kino2001/biylineapi
