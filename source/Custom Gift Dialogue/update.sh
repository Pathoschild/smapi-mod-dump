cat mod.list | grep -e '^\w' | sed -e 's/^[[:space:]]*(.*)[[:space:]]*$/$1/' | while read -r repo; do
   (
      echo "> Updating repo $repo ...";
      cd $repo && git pull;
   )
done
